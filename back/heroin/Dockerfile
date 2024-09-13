# 1단계: Gradle과 JDK 17을 사용하여 빌드
FROM gradle:8.3-jdk17 AS build
WORKDIR /app

# Gradle 캐싱을 위한 설정
COPY build.gradle settings.gradle ./
RUN gradle build --no-daemon || true

# 애플리케이션 소스를 복사하고 빌드
COPY src ./src
COPY src/main/resources/application-secret.yml /app/config/application-secret.yml
RUN gradle clean build --no-daemon -x test

# 빌드된 JAR 파일 목록 확인
RUN ls -l /app/build/libs/

# 2단계: 최종 실행 이미지 (JRE 17만 포함, 필요 시 JDK로 변경)
FROM openjdk:17-jdk-slim
WORKDIR /app

# 빌드된 jar 파일을 복사 (정확한 파일명 사용 권장)
COPY --from=build /app/build/libs/*.jar app.jar

# 빌드 단계에서 복사한 설정 파일을 최종 이미지로 복사
COPY --from=build /app/config /app/config

# 애플리케이션 실행 명령
ENTRYPOINT ["java", "-jar", "app.jar", "--spring.config.additional-location=/app/config/application-secret.yml"]
