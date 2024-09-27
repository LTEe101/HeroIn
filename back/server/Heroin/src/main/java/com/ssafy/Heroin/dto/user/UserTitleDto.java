package com.ssafy.Heroin.dto.user;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;

import java.time.LocalDateTime;

@Getter
@Setter
public class UserTitleDto {

    public String userTitle;
    public LocalDateTime createAt;
}
