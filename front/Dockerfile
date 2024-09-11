# 1. Node.js를 사용해 빌드
FROM node:16 AS build

# 작업 디렉토리 설정
WORKDIR /app

# 의존성 설치
COPY package.json ./
COPY package-lock.json ./
RUN npm install

# 애플리케이션 빌드
COPY . .
RUN npm run build

# 2. Nginx를 사용해 정적 파일 서빙
FROM nginx:alpine

# 빌드된 리액트 파일들을 Nginx 기본 경로에 복사
COPY --from=build /app/build /usr/share/nginx/html

# Nginx 컨테이너가 실행될 포트
EXPOSE 80

# Nginx 시작 명령
CMD ["nginx", "-g", "daemon off;"]
