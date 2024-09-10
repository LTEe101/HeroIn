# 단계 1: Node.js를 사용하여 React 애플리케이션을 빌드
FROM node:18 AS build

# 작업 디렉토리 설정
WORKDIR /app

# package.json과 package-lock.json을 복사
COPY package.json package-lock.json ./

# 의존성 설치
RUN npm install

# 소스 코드 복사
COPY . .

# React 애플리케이션 빌드
RUN npm run build

# 단계 2: Nginx를 사용하여 빌드된 애플리케이션을 서비스
FROM nginx:alpine

# Nginx의 기본 HTML 디렉토리로 빌드된 파일 복사
COPY --from=build /app/build /usr/share/nginx/html

# Nginx 기본 포트(80) 노출
EXPOSE 80

# Nginx 실행
CMD ["nginx", "-g", "daemon off;"]
