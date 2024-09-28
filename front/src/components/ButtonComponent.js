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

// Styled Components for the Button
const DownloadButton = styled.button`
  font-family: "SF_HambakSnow", serif;
  background-color: black; // 버튼 배경을 검은색으로 변경
  border: 2px solid black;
  border-radius: 5px;
  padding: 15px 40px;
  color: white; // 글씨는 흰색으로 변경
  font-size: 1.5em; // 글씨 크기를 크게 조정
  letter-spacing: 1px;
  cursor: pointer;
  transition: background-color 0.3s, color 0.3s;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);

  &:hover {
    background-color: white; // 호버 시 배경은 흰색으로
    color: black; // 호버 시 글씨는 검은색으로
  }
`;

const ButtonComponent = ({ text, onClick }) => {
  return (
    <>
      <GlobalStyle />
      <DownloadButton onClick={onClick}>{text}</DownloadButton>
    </>
  );
};

export default ButtonComponent;
