import cv2
from cvzone.HandTrackingModule import HandDetector
import socket
import numpy as np

buffer = {}

# 손을 감지
detector = HandDetector(maxHands=2, detectionCon=0.8)

# 네트워크 설정
recv_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
send_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

recv_address = ("127.0.0.1", 5052)
send_address = ("127.0.0.1", 5053)

# 새로운 포트 번호 사용
recv_sock.bind(recv_address)

while True:
    data, addr = recv_sock.recvfrom(65507)

    packet_number = int.from_bytes(data[:4], byteorder='little')
    packet_data = data[4:]

    #print(f'packet_number: {packet_number}')

    if packet_number not in buffer:
        buffer[packet_number] = packet_data

    # 모든 패킷이 도착했는지 확인
    if len(buffer) == max(buffer.keys()) + 1:
        full_data = b''.join([buffer[i] for i in sorted(buffer.keys())])
        buffer.clear()

        # 바이트 데이터를 numpy 배열로 변환
        nparr = np.frombuffer(full_data, np.uint8)

        # numpy 배열을 이미지로 디코딩, 이미지를 좌우 반전
        frame = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        frame = cv2.flip(frame, 1)

        # 만약 이미지가 2차원이라면, 3차원으로 변환
        if len(frame.shape) == 2:
            frame = cv2.cvtColor(frame, cv2.COLOR_GRAY2BGR)

        # Hands 감지 및 이미지 반환
        hands, frame = detector.findHands(frame)

        # 이미지의 가로 크기 가져오기 (좌우 반전 후)
        img_width = frame.shape[1]

        # 각 손에 대한 데이터를 저장할 딕셔너리 초기화
        hand_data = {'Left': 'NoData', 'Right': 'NoData'}

        if hands:
            for hand in hands:
                data = []  # 각 손에 대한 데이터를 따로 저장

                # 경계 박스 정보 가져오기
                bbox = hand['bbox']  # x, y, w, h
                x, y, w, h = bbox

                # 각 랜드마크의 좌표 가져오기
                lmList = hand['lmList']  # 각 랜드마크의 x, y, z 좌표 리스트
                for idx, lm in enumerate(lmList):
                    cx, cy, cz = lm

                    # 좌표를 반전하여 저장
                    data.extend([-lm[0], img_width - lm[1], lm[2]])

                    # 랜드마크 좌표 표시
                    cv2.circle(frame, (cx, cy), 5, (0, 0, 255), cv2.FILLED)
                    cv2.putText(frame, f'{idx}', (cx + 10, cy), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 255, 255), 1)

                # 손의 타입 (왼손/오른손)을 추가하여 데이터를 저장
                handType = hand["type"]  # "Left" 또는 "Right"
                hand_data[handType] = data  # 데이터 저장

        # 각 손의 데이터를 전송
        for handType in ['Left', 'Right']:
            data = hand_data[handType]
            if data == 'NoData':
                message = f'{handType}:NoData'
            else:
                message = f'{handType}:{data}'
            send_sock.sendto(str.encode(message), send_address)

        cv2.imshow("Image", frame)

        if cv2.waitKey(1) == ord("q"):  # q 누를 시 웹캠 종료
            break

frame.release()
cv2.destroyAllWindows()
recv_sock.close()
send_sock.close()
