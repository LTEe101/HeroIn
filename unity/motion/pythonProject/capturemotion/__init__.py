import cv2
from cvzone.HandTrackingModule import HandDetector
import socket

# WebCam 사용할 경우
cap = cv2.VideoCapture(0)

# 손을 감지
detector = HandDetector(maxHands=2, detectionCon=0.8) 

# 네트워크
sock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

while True:  # 웹캠에서 프레임 가져오기
    success, img = cap.read()

    # 이미지를 좌우 반전
    img = cv2.flip(img, 1)

    # Hands 감지 및 이미지 반환
    hands, img = detector.findHands(img)

    # 이미지의 가로 크기 가져오기 (좌우 반전 후)
    img_width = img.shape[0]

    data = []

    # 손이 감지된 경우
    if hands:
        for hand in hands:
            # 경계 박스 정보 가져오기
            bbox = hand['bbox']  # x, y, w, h
            x, y, w, h = bbox
            # 경계 박스 크기 표시
            cv2.rectangle(img, (x, y), (x + w, y + h), (0, 255, 0), 2)
            cv2.putText(img, f'Width: {w}, Height: {h}', (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 0, 0), 2)

            # 각 랜드마크의 좌표 가져오기
            lmList = hand['lmList']  # 각 랜드마크의 x, y, z 좌표 리스트
            for idx, lm in enumerate(lmList):
                cx, cy, cz = lm

                # 랜드마크 좌표 표시
                cv2.circle(img, (cx, cy), 5, (0, 0, 255), cv2.FILLED)
                cv2.putText(img, f'{idx}', (cx + 10, cy), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 255, 255), 1)
                data.extend([-lm[0], img_width - lm[1], lm[2]])

                # 좌표를 콘솔에 출력 (반전된 좌표로 출력)
                #print(f'Landmark {idx}: x: {cx}, y: {cy}, z: {cz}')
            print(data)
            print(len(data))
            sock.sendto(str.encode(str(data)), serverAddressPort)

    cv2.imshow("Image", img)

    if cv2.waitKey(1) == ord("q"):  # q 누를 시 웹캠 종료
        break

cap.release()
cv2.destroyAllWindows()
sock.close()
