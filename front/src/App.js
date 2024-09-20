import React from "react";
import styled, { createGlobalStyle } from "styled-components";
import GlitchButtonComponent from "./components/GlitchButtonComponent";

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

// Styled Components
const GameContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh; /* 화면 전체 높이 */
  text-align: center; /* 텍스트 중앙 정렬 */
  color: white; /* 텍스트 색상 흰색으로 변경 */

  /* 배경 이미지 + 그라데이션 */
  background: linear-gradient(rgba(0, 0, 0, 0.8), rgba(255, 255, 255, 0.4)),
    url(${require("./assets/images/background.png")}); /* 이미지 경로 */
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
`;

const Logo = styled.img`
  width: 200px;
  margin-bottom: 20px;
`;

const VideoContainer = styled.div`
  width: 80%;
  max-width: 600px;
  margin: 20px 0;

  iframe {
    border: none; /* 테두리 없애기 */
  }
`;

const App = () => {
  const handleDownloadClick = (event) => {
    event.preventDefault(); // 기본 동작 방지
    const confirmDownload = window.confirm("정말로 다운로드하시겠습니까?");

    if (confirmDownload) {
      window.location.href = "/downloads/heroin.zip"; // 다운로드 링크
    }
  };

  return (
    <>
      <GlobalStyle />
      <GameContainer>
        <Logo src={require("./assets/images/logo.png")} alt="Hero-in Logo" />
        <h1>
          스토리텔링과 미니게임으로 배우는 재밌는 한국사 <br />
          역사 속 영웅이 되어보세요!
        </h1>
        <VideoContainer>
          <iframe
            width="100%"
            height="315"
            src="https://www.youtube.com/embed/8owhox1bPWw?autoplay=1&mute=1"
            title="YouTube video player"
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowFullScreen></iframe>
        </VideoContainer>
        <GlitchButtonComponent
          text="게임 다운로드"
          onClick={handleDownloadClick}
        />
      </GameContainer>
    </>
  );
};

export default App;
