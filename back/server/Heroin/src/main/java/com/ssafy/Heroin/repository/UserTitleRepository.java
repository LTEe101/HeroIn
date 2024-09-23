package com.ssafy.Heroin.repository;

import com.ssafy.Heroin.domain.UserTitle;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.util.List;

public interface UserTitleRepository extends JpaRepository<UserTitle, Long> {

    @Query("SELECT t FROM UserTitle t JOIN FETCH t.user u WHERE u.userId = :userId")
    List<UserTitle> findByUserId(String userId);
}
