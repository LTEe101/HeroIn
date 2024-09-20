import React from "react";
import styled from "styled-components";

// Styled Components
const GameContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh; /* 화면 전체 높이를 사용 */
  text-align: center; /* 텍스트 중앙 정렬 */
`;

const Logo = styled.img`
  width: 200px; /* 로고 너비 조정 */
  margin-bottom: 20px; /* 로고와 문구 사이 간격 */
`;

const VideoContainer = styled.div`
  width: 80%; /* 동영상 컨테이너 너비 */
  max-width: 600px; /* 최대 너비 제한 */
  margin: 20px 0; /* 상하 여백 */
`;

const DownloadButton = styled.button`
  padding: 10px 20px;
  background-color: #4caf50; /* 초록색 배경 */
  color: white; /* 흰색 글씨 */
  border: none;
  cursor: pointer;
  font-size: 16px;
  margin-top: 20px;
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
    <GameContainer>
      {/* 로고 이미지 추가 */}
      <Logo src={require("./assets/images/logo.png")} alt="Hero-in Logo" />
      <h1>
        스토리텔링과 미니게임으로 배우는 재밌는 한국사 <br />
        역사 속 영웅이 되어보세요!
      </h1>
      <VideoContainer>
        <iframe
          width="100%"
          height="315"
          src="https://www.youtube.com/embed/8owhox1bPWw?autoplay=1&mute=1" // 여기에 올바른 VIDEO_ID를 넣으세요
          title="YouTube video player"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          allowFullScreen></iframe>
      </VideoContainer>
      <DownloadButton type="button" onClick={handleDownloadClick}>
        게임 다운로드
      </DownloadButton>
    </GameContainer>
  );
};

export default App;
