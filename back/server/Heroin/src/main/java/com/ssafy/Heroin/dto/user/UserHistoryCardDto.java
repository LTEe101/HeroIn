package com.ssafy.Heroin.dto.user;

import com.ssafy.Heroin.domain.HistoryCard;
import com.ssafy.Heroin.domain.User;
import lombok.Getter;
import lombok.Setter;

import java.time.LocalDateTime;

@Getter @Setter
public class UserHistoryCardDto {

    private String username;
    private String CardName;
    private String CardDescription;
    private String CardImg;
    private LocalDateTime creationDate;

}
