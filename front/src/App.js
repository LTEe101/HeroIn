import React, { useEffect, useRef } from "react";
import styled, { createGlobalStyle } from "styled-components";
import ButtonComponent from "./components/ButtonComponent"; // 기존 GlitchButton 대신 전통 버튼 컴포넌트로 변경

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

const Logo = styled.img`
  width: 200px;
  margin-bottom: 20px;
`;

const GameContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh; /* 화면 전체 높이 */
  text-align: center; /* 텍스트 중앙 정렬 */
  color: white; /* 텍스트 색상 흰색으로 변경 */

  /* 배경 이미지 + 그라데이션 */
  background: linear-gradient(rgba(0, 0, 0, 0.4), rgba(255, 255, 255, 0.4)),
    url(${require("./assets/images/background.png")}); /* 이미지 경로 */
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;

  /* 그림자 추가 */
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.5); /* 그림자 효과 */
  padding: 20px; /* 여백 추가 */
`;

const VideoContainer = styled.div`
  width: 80%;
  max-width: 550px;
  margin: 20px 0;

  /* 모서리를 둥글게 만들기 */
  border-radius: 30px;

  /* 그림자 제거 */
  box-shadow: none;

  iframe {
    border: none; /* 테두리 없애기 */
    width: 100%;
    border-radius: 30px; /* iframe 내부도 둥글게 만들기 */
  }
`;

const App = () => {
  const videoRef = useRef(null);

  useEffect(() => {
    const script = document.createElement("script");
    script.src = "https://www.youtube.com/iframe_api";
    document.body.appendChild(script);

    window.onYouTubeIframeAPIReady = () => {
      const player = new window.YT.Player(videoRef.current, {
        events: {
          onStateChange: (event) => {
            if (event.data === window.YT.PlayerState.ENDED) {
              player.playVideo(); // 비디오가 종료되면 재생
            }
          },
        },
      });
    };

    return () => {
      // 클린업
      document.body.removeChild(script);
    };
  }, []);

  const handleDownloadClick = (event) => {
    event.preventDefault(); // 기본 동작 방지
    console.log("클릭됨"); // 확인용 로그
    const confirmDownload = window.confirm("정말로 다운로드하시겠습니까?");

    if (confirmDownload) {
      window.location.href = "https://j11e101.p.ssafy.io/downloads/heroin.exe"; // 다운로드 링크
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
            ref={videoRef}
            width="100%"
            height="315"
            src="https://www.youtube.com/embed/8owhox1bPWw?enablejsapi=1&autoplay=1&mute=1"
            title="YouTube video player"
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowFullScreen></iframe>
        </VideoContainer>
        <ButtonComponent text="다운로드" onClick={handleDownloadClick} />
      </GameContainer>
    </>
  );
};

export default App;
