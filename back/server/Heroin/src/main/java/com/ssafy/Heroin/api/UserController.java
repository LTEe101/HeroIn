package com.ssafy.Heroin.api;

import com.ssafy.Heroin.dto.jwt.JwtToken;
import com.ssafy.Heroin.dto.user.SignInDto;
import com.ssafy.Heroin.dto.user.SignUpDto;
import com.ssafy.Heroin.dto.user.UserDto;
import com.ssafy.Heroin.dto.user.UserProfileDto;
import com.ssafy.Heroin.service.UserService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@Slf4j
@RequiredArgsConstructor
@RestController
@RequestMapping("/api/user")
public class UserController {

    private final UserService userService;

    @PostMapping("/login")
    public JwtToken signIn(@RequestBody SignInDto signInDto) {
        System.out.println("hello");
        String username = signInDto.getUserLoginId();
        String password = signInDto.getUserLoginPw();
        JwtToken jwtToken = userService.signIn(username, password);
        log.info("request username = {}, password = {}", username, password);
        log.info("jwtToken accessToken = {}, refreshToken = {}", jwtToken.getAccessToken(), jwtToken.getRefreshToken());
        return jwtToken;
    }

    @PostMapping("/sign-up")
    public ResponseEntity<?> signUp(@RequestBody UserDto userDto) {
        SignUpDto signUpDto = userService.signUp(userDto);
        return ResponseEntity.ok(signUpDto);
    }

    @GetMapping("/profile")
    public ResponseEntity<?> getProfile(@RequestBody UserDto userDto) {
        UserProfileDto userProfile = userService.getUserProfile(userDto);

        if (userProfile != null) {
            return ResponseEntity.ok(userProfile);
        }

        return ResponseEntity.notFound().build();
    }
}
