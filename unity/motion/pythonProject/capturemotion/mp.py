import cv2
from cvzone.HandTrackingModule import HandDetector
import socket

# WebCam 사용할 경우
cap = cv2.VideoCapture(0)

# 손을 감지
detector = HandDetector(maxHands=2, detectionCon=0.8)

# 네트워크 설정
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

while True:
    # 웹캠에서 프레임 가져오기
    success, img = cap.read()

    # 이미지를 좌우 반전
    img = cv2.flip(img, 1)

    # Hands 감지 및 이미지 반환
    hands, img = detector.findHands(img)

    # 이미지의 가로 크기 가져오기 (좌우 반전 후)
    img_width = img.shape[1]

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
                cv2.circle(img, (cx, cy), 5, (0, 0, 255), cv2.FILLED)
                cv2.putText(img, f'{idx}', (cx + 10, cy), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 255, 255), 1)

            # 손의 타입 (왼손/오른손)을 추가하여 데이터를 전송
            handType = hand["type"]  # "Left" 또는 "Right"
            print(f'{handType} Hand Data: {data}')
            print(f'{handType} Data Length: {len(data)}')
            sock.sendto(str.encode(f'{handType}:{data}'), serverAddressPort)

    cv2.imshow("Image", img)

    if cv2.waitKey(1) == ord("q"):  # q 누를 시 웹캠 종료
        break

cap.release()
cv2.destroyAllWindows()
sock.close()
