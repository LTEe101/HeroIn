package com.ssafy.Heroin.service;

import com.ssafy.Heroin.domain.*;
import com.ssafy.Heroin.dto.jwt.JwtToken;
import com.ssafy.Heroin.dto.user.*;
import com.ssafy.Heroin.jwt.JwtTokenProvider;
import com.ssafy.Heroin.repository.*;
import io.jsonwebtoken.Claims;
import lombok.RequiredArgsConstructor;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.config.annotation.authentication.builders.AuthenticationManagerBuilder;
import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.multipart.MultipartFile;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@RequiredArgsConstructor
@Transactional
@Service
public class UserService {

    private final UserRepository userRepository;
    private final AuthenticationManagerBuilder authenticationManagerBuilder;
    private final JwtTokenProvider jwtTokenProvider;
    private final UserHistoryCardRepository userHistoryCardRepository;
    private final UserTitleRepository userTitleRepository;
    private final HistoryCardRepository historyCardRepository;
    private final TitleRepository titleRepository;
    private final S3UploadService s3UploadService;

    @Transactional
    public JwtToken signIn(String username, String password) {
        // 1. username + password 를 기반으로 Authentication 객체 생성
        // 이때 authentication 은 인증 여부를 확인하는 authenticated 값이 false
        UsernamePasswordAuthenticationToken authenticationToken = new UsernamePasswordAuthenticationToken(username, password);
        System.out.println(authenticationToken);

        // 2. 실제 검증. authenticate() 메서드를 통해 요청된 User 에 대한 검증 진행
        // authenticate 메서드가 실행될 때 CustomUserDetailsService 에서 만든 loadUserByUsername 메서드 실행
        Authentication authentication = authenticationManagerBuilder.getObject().authenticate(authenticationToken);
        System.out.println(authentication);

        // 3. 인증 정보를 기반으로 JWT 토큰 생성
        return jwtTokenProvider.generateToken(authentication);
    }

    public SignUpDto signUp(MultipartFile userImg, UserDto userDto) {

        String userImgLink = s3UploadService.upload(userImg);
        User user = DtotoUser(userDto, userImgLink);
        ExistUser(user.getUserId());
        user.setHistoryCards(null);
        userRepository.save(user);
        SignUpDto signUpDto = new SignUpDto();
        signUpDto.setUserLoginId(user.getUserId());
        signUpDto.setUserLoginPw(user.getUserPw());
        signUpDto.setUserName(user.getUsername());
        signUpDto.setUserImg(userImgLink);

        return signUpDto;
    }

    public void addHistoryCardandTitle(Long checkId, String userId) {
        UserHistoryCard userHistoryCard = new UserHistoryCard();
        UserTitle userTitle = new UserTitle();

        Optional<User> Ouser = userRepository.findByUserId(userId);
        User user = new User();
        if(Ouser.isPresent()) {
            user = Ouser.get();
            userHistoryCard.setUser(user);
            userTitle.setUser(user);
        }
        Optional<HistoryCard> historyCard = historyCardRepository.findById(checkId);
        if(historyCard.isPresent()) {
            userHistoryCard.setHistoryCard(historyCard.get());
        }
        Optional<Title> title = titleRepository.findById(checkId);
        if (title.isPresent()) {
            userTitle.setTitle(title.get());
        }

        userHistoryCardRepository.save(userHistoryCard);
        userTitleRepository.save(userTitle);
    }

    private User DtotoUser(UserDto userDto, String userImgLink) {
        User user = new User();

        user.setUserId(userDto.getUserLoginId());
        user.setUserPw(userDto.getUserLoginPw());
        user.setUsername(userDto.getUserName());
        user.setImgNo(userImgLink);

        return user;
    }

    private void ExistUser(String userLoginId) {

        if(userRepository.existsByUserId(userLoginId)) {
            throw new IllegalStateException("이미 존재하는 회원입니다.");
        }
    }

    @Transactional
    public UserProfileDto getUserProfile(JwtToken token) {
        Claims claim = jwtTokenProvider.getPayload(token.getAccessToken());
        Optional<User> user = userRepository.findByUserId(claim.getSubject());

        if(user.isPresent()) {
            UserProfileDto userProfileDto = new UserProfileDto();
            userProfileDto.setUserName(user.get().getUsername());
            userProfileDto.setImg(user.get().getImgNo());
            userProfileDto.setTitle(user.get().getTitle());

            return userProfileDto;
        }

        return null;
    }

    @Transactional
    public List<UserHistoryCardDto> getHistoryCards(String userId) {
        List<UserHistoryCard> userHistoryCards = userHistoryCardRepository.findByUser(userId);

        List<UserHistoryCardDto> userHistoryCardDtos = new ArrayList<>();

        if(!userHistoryCards.isEmpty()) {

            for(UserHistoryCard userHistoryCard : userHistoryCards) {
                UserHistoryCardDto userHistoryCardDto = new UserHistoryCardDto();
                userHistoryCardDto.setCardName(userHistoryCard.getHistoryCard().getName());
                userHistoryCardDto.setCardDescription(userHistoryCard.getHistoryCard().getDescription());
                userHistoryCardDto.setCardImg(userHistoryCard.getHistoryCard().getImgNo());
                userHistoryCardDto.setUsername(userHistoryCard.getUser().getUsername());
                userHistoryCardDto.setCreationDate(userHistoryCard.getCreateAt());

                userHistoryCardDtos.add(userHistoryCardDto);
            }

            return userHistoryCardDtos;
        }

        return null;
    }

    @Transactional
    public List<UserTitleDto> getTitles(String userId) {
        List<UserTitle> userTitles = userTitleRepository.findByUserId(userId);
        List<UserTitleDto> userTitleDtos = new ArrayList<>();

        if(!userTitles.isEmpty()) {
            for(UserTitle userTitle : userTitles) {
                UserTitleDto userTitleDto = new UserTitleDto();
                userTitleDto.setUserTitle(userTitle.getTitle().getTitle());
                userTitleDto.setCreateAt(userTitle.getCreateAt());
                userTitleDtos.add(userTitleDto);
            }

            return userTitleDtos;
        }

        return null;
    }
}
