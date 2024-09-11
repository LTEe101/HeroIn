import "./App.css";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1>파일 다운로드</h1>
        <p>아래 링크를 클릭하여 파일을 다운로드하세요.</p>

        <li>
          <a
            className="App-link"
            href="/downloads/heroin.zip"
            target="_blank"
            rel="noopener noreferrer"
            download>
            HeroIn 다운로드
          </a>
        </li>
        <h2>CI/CD 테스트</h2>
      </header>
    </div>
  );
}

export default App;
