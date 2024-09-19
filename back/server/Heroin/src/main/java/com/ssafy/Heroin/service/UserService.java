package com.ssafy.Heroin.service;

import com.ssafy.Heroin.domain.User;
import com.ssafy.Heroin.dto.jwt.JwtToken;
import com.ssafy.Heroin.dto.user.SignUpDto;
import com.ssafy.Heroin.dto.user.UserDto;
import com.ssafy.Heroin.jwt.JwtTokenProvider;
import com.ssafy.Heroin.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.config.annotation.authentication.builders.AuthenticationManagerBuilder;
import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@RequiredArgsConstructor
@Transactional(readOnly = true)
@Service
public class UserService {

    private final UserRepository userRepository;
    private final AuthenticationManagerBuilder authenticationManagerBuilder;
    private final JwtTokenProvider jwtTokenProvider;

    @Transactional
    public JwtToken signIn(String username, String password) {
        // 1. username + password 를 기반으로 Authentication 객체 생성
        // 이때 authentication 은 인증 여부를 확인하는 authenticated 값이 false
        UsernamePasswordAuthenticationToken authenticationToken = new UsernamePasswordAuthenticationToken(username, password);
        System.out.println(authenticationToken);

        // 2. 실제 검증. authenticate() 메서드를 통해 요청된 Member 에 대한 검증 진행
        // authenticate 메서드가 실행될 때 CustomUserDetailsService 에서 만든 loadUserByUsername 메서드 실행
        Authentication authentication = authenticationManagerBuilder.getObject().authenticate(authenticationToken);
        System.out.println(authentication);

        // 3. 인증 정보를 기반으로 JWT 토큰 생성
        return jwtTokenProvider.generateToken(authentication);
    }

    public SignUpDto signUp(UserDto userDto) {
        User user = DtotoUser(userDto);
        ExistUser(user.getUserId());
        userRepository.save(user);
        SignUpDto signUpDto = new SignUpDto();
        signUpDto.setUserLoginId(user.getUserId());
        signUpDto.setUserLoginPw(user.getUserPw());
        signUpDto.setUserName(user.getUsername());

        return signUpDto;
    }

    private User DtotoUser(UserDto userDto) {
        User user = new User();

        user.setUserId(userDto.getUserLoginId());
        user.setUserPw(userDto.getUserLoginPw());
        user.setUsername(userDto.getUserName());

        return user;
    }

    private void ExistUser(String userLoginId) {

        if(userRepository.existsByUserId(userLoginId)) {
            throw new IllegalStateException("이미 존재하는 회원입니다.");
        }
    }
}
