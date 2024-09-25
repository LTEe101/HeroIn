import React from "react";
import styled, { createGlobalStyle } from "styled-components";

// Global Style with font-face
const GlobalStyle = createGlobalStyle`
  @font-face {
    font-family: 'SF_HambakSnow';
    src: url('https://fastly.jsdelivr.net/gh/projectnoonnu/noonfonts_2106@1.1/SF_HambakSnow.woff') format('woff');
    font-weight: normal;
    font-style: normal;
  }

  body {
    font-family: 'SF_HambakSnow', sans-serif;
  }
`;

// Styled Components for the traditional Korean style Button
const TraditionalButton = styled.button`
  font-family: "SF_HambakSnow", serif; // 요청하신 폰트 적용
  background-color: #8b4513; // 나무 느낌의 배경색
  border: 3px solid #333; // 두꺼운 테두리
  border-radius: 5px; // 부드러운 모서리
  padding: 20px 40px; // 버튼 크기 키움
  color: #fff;
  font-size: 1.5em; // 폰트 크기 키움
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.3);
  text-align: center;
  position: relative;
  cursor: pointer;

  // 버튼 텍스트 스타일
  &:hover {
    background-color: #6e3b09; // 어두운 나무색으로 변환
    box-shadow: 0 8px 12px rgba(0, 0, 0, 0.5);
  }

  // 푸른색 기와 지붕 느낌을 표현하기 위한 상단 요소
  &::before {
    content: "";
    display: block;
    position: absolute;
    top: -15px; // 버튼 상단에서 살짝 떨어지게 설정
    left: -20px; // 버튼 좌우로 기와가 튀어나가게 설정
    right: -20px;
    height: 25px;
    background: repeating-linear-gradient(
      90deg,
      #2c7a7b,
      // 푸른색 기와
      #2c7a7b 10px,
      #2e86ab 10px,
      #2e86ab 20px
    );
    border-top-left-radius: 15px;
    border-top-right-radius: 15px;
  }
`;

const ButtonComponent = ({ text, onClick }) => {
  return (
    <>
      <GlobalStyle />
      <TraditionalButton onClick={onClick}>{text}</TraditionalButton>
    </>
  );
};

export default ButtonComponent;
