import React, { useEffect, useRef } from "react";
import styled from "styled-components";

// Styled Components for the Button
const Button = styled.button`
  --bevel: 3px;
  --border-width: 3px;
  font-family: "DIN 2014";
  font-weight: 600;
  color: #1d2119;
  filter: drop-shadow(1px 1px 1px rgba(0, 0, 0, 0.95));
  min-width: 10em;
  background: none;
  border: none;
  cursor: pointer;
  position: relative;
  padding: 0;
  display: inline-block;

  .button-outline {
    --bevel-1: calc(
      (var(--bevel) + (var(--border-width)) * 2) -
        ((var(--border-width) * 0.41421)) * 2
    );
    --bevel-2: calc(var(--bevel-1) + var(--border-width));
    --bevel-3: calc(var(--bevel-2) + var(--border-width));
    display: block;
    margin-top: calc(var(--border-width) * -1);
    margin-left: calc(var(--border-width) * -1);
    padding: var(--border-width);
    background-color: #fff;
    clip-path: polygon(
      var(--bevel-2) var(--border-width),
      calc(100% - var(--bevel-2)) var(--border-width),
      100% var(--bevel-3),
      100% calc(100% - var(--bevel-1)),
      calc(100% - var(--bevel-1)) 100%,
      var(--bevel-3) 100%,
      var(--border-width) calc(100% - var(--bevel-2)),
      var(--border-width) var(--bevel-2)
    );
    transition-property: clip-path;
    transition-duration: 0.2s;
  }

  &:hover:not(:active) .button-outline {
    clip-path: polygon(
      var(--bevel-1) 0,
      calc(100% - var(--bevel-3)) 0,
      100% var(--bevel-3),
      100% calc(100% - var(--bevel-1)),
      calc(100% - var(--bevel-1)) 100%,
      var(--bevel-3) 100%,
      0 calc(100% - var(--bevel-3)),
      0 var(--bevel-1)
    );
  }

  .button-shadow {
    --padding: calc(var(--border-width) * 2);
    --bevel-1: calc(
      (var(--bevel) + var(--border-width)) - (var(--border-width) * 0.41421)
    );
    --bevel-2: calc(var(--bevel-1) + var(--border-width));
    --bevel-3: calc(var(--bevel-2) + var(--border-width));
    display: block;
    padding: calc(var(--border-width) * 2) var(--padding) var(--padding)
      calc(var(--border-width) * 2);
    background-color: #1d2119;
    clip-path: polygon(
      var(--bevel-2) var(--border-width),
      calc(100% - var(--bevel-2)) var(--border-width),
      100% var(--bevel-3),
      100% calc(100% - var(--bevel-1)),
      calc(100% - var(--bevel-1)) 100%,
      var(--bevel-3) 100%,
      var(--border-width) calc(100% - var(--bevel-2)),
      var(--border-width) var(--bevel-2)
    );
    transition-property: clip-path;
    transition-duration: 0.2s;
  }

  &:hover:not(:active) .button-shadow {
    clip-path: polygon(
      var(--bevel-1) 0,
      calc(100% - var(--bevel-3)) 0,
      100% var(--bevel-3),
      100% calc(100% - var(--bevel-1)),
      calc(100% - var(--bevel-1)) 100%,
      var(--bevel-3) 100%,
      0 calc(100% - var(--bevel-3)),
      0 var(--bevel-1)
    );
  }

  .button-inside {
    --padding-vertical: 6px;
    display: block;
    padding: var(--padding-vertical) 24px
      calc(var(--padding-vertical) - 0.125em);
    background-color: #fff;
    clip-path: polygon(
      var(--bevel) 0,
      calc(100% - var(--bevel)) 0,
      100% var(--bevel),
      100% calc(100% - var(--bevel)),
      calc(100% - var(--bevel)) 100%,
      var(--bevel) 100%,
      0 calc(100% - var(--bevel)),
      0 var(--bevel)
    );
    text-align: center;
    transition-property: transform;
    transition-duration: 0.2s;
  }

  &:hover:not(:active) .button-inside {
    transform: translate(
      calc(var(--border-width) * -1),
      calc(var(--border-width) * -1)
    );
  }

  &:hover .button-inside {
    background-color: #fcd200;
    background-image: linear-gradient(
        to right,
        rgba(0, 0, 0, 0),
        rgba(252, 210, 0, 0.9)
      ),
      radial-gradient(#fff60d 1px, rgba(0, 0, 0, 0) 0%),
      radial-gradient(#fff60d 1px, rgba(0, 0, 0, 0) 0%);
    background-size: auto, 6px 6px, 6px 6px;
    background-position: 0 0, 0 0, 3px 3px;
    animation: scroll-background 1s linear infinite;
  }

  @keyframes scroll-background {
    to {
      background-position-x: 0, -6px, -3px;
    }
  }

  .button-text-characters-container {
    display: inline-block;
    transform: skewX(-6deg);
  }

  .button-text-character {
    display: inline-block;
  }

  &:hover:not(:active) .button-text-character {
    animation: jump 4s cubic-bezier(0.75, 0.25, 1, 2) var(--delay) infinite;
  }

  @keyframes jump {
    5% {
      transform: translateY(-0.125em);
    }
    10% {
      transform: translateY(0);
    }
  }
`;

const ButtonComponent = ({ text }) => {
  const buttonTextRef = useRef(null);

  useEffect(() => {
    const buttonText = buttonTextRef.current;
    const charactersContainer = buttonText.querySelector(
      ".button-text-characters-container"
    );

    // 텍스트가 중복되지 않도록 기존의 자식 요소들을 모두 비움
    charactersContainer.innerHTML = "";

    const characters = text.split("");

    const characterCountWithoutWhitespaces = characters.filter(
      (character) => !/\s/.test(character)
    ).length;

    let characterIndex = 1;

    characters.forEach((character) => {
      const span = document.createElement("span");
      span.textContent = character;

      if (!/\s/.test(character)) {
        span.classList.add("button-text-character");
        const delay = `calc(2s / ${characterCountWithoutWhitespaces} * ${characterIndex} + 1s)`;
        span.style.setProperty("--delay", delay);

        characterIndex++;
      }

      charactersContainer.appendChild(span);
    });
  }, [text]);

  return (
    <Button className="button">
      <span className="button-outline">
        <span className="button-shadow">
          <span className="button-inside" ref={buttonTextRef}>
            <span className="button-text-characters-container"></span>
          </span>
        </span>
      </span>
    </Button>
  );
};

export default ButtonComponent;
