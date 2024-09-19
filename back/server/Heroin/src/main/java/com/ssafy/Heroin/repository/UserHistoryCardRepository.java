package com.ssafy.Heroin.repository;

import com.ssafy.Heroin.domain.UserHistoryCard;
import com.ssafy.Heroin.dto.user.UserDto;
import com.ssafy.Heroin.dto.user.UserHistoryCardDto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.util.List;

public interface UserHistoryCardRepository extends JpaRepository<UserHistoryCard, Long> {

    @Query("SELECT c " +
            " FROM UserHistoryCard c" +
            " WHERE (c.user.userId LIKE :#{#userDto.userLoginId})")
    List<UserHistoryCardDto> findByUser(UserDto userDto);
}
