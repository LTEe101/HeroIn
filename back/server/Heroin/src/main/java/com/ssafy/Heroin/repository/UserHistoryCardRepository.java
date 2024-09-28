package com.ssafy.Heroin.repository;

import com.ssafy.Heroin.domain.UserHistoryCard;
import com.ssafy.Heroin.dto.user.UserHistoryCardDto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.util.List;

public interface UserHistoryCardRepository extends JpaRepository<UserHistoryCard, Long> {

    @Query("SELECT c FROM UserHistoryCard c JOIN FETCH c.user u WHERE u.userId = :userId")
    List<UserHistoryCard> findByUser(String userId);
}
