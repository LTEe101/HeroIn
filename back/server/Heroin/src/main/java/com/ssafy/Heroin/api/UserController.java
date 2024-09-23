package com.ssafy.Heroin.api;

import com.ssafy.Heroin.dto.jwt.JwtToken;
import com.ssafy.Heroin.dto.user.*;
import com.ssafy.Heroin.service.UserService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@Slf4j
@RequiredArgsConstructor
@RestController
@RequestMapping("/api/user")
public class UserController {

    private final UserService userService;

    @PostMapping("/login")
    public JwtToken signIn(@RequestBody SignInDto signInDto) {
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

    @PostMapping("/profile")
    public ResponseEntity<?> getProfile(@RequestBody JwtToken token) {
        UserProfileDto userProfile = userService.getUserProfile(token);

        if (userProfile != null) {
            return ResponseEntity.ok(userProfile);
        }

        return ResponseEntity.notFound().build();
    }

    @GetMapping("/card")
    public ResponseEntity<?> getUserCard(@RequestParam String userId) {
        List<UserHistoryCardDto> historyCards = userService.getHistoryCards(userId);

        return ResponseEntity.ok(historyCards);
    }

    @GetMapping("/title")
    public ResponseEntity<?> getUserTitle(@RequestParam String userId) {
        List<UserTitleDto> titleDtos = userService.getTitles(userId);
        System.out.println(titleDtos);

        return ResponseEntity.ok(titleDtos);
    }
}
