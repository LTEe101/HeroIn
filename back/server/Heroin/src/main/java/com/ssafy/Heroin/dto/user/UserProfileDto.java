package com.ssafy.Heroin.dto.user;

import com.ssafy.Heroin.domain.UserHistoryCard;
import lombok.Getter;
import lombok.Setter;

import java.util.List;

@Getter @Setter
public class UserProfileDto {

    private String userName;
    private String img;
    private String title;
}
