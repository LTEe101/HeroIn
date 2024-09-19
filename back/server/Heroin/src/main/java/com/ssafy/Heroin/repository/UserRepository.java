package com.ssafy.Heroin.repository;

import com.ssafy.Heroin.domain.User;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface UserRepository extends JpaRepository<User, Integer> {

    Optional<User> findByUserId(String userId);
    boolean existsByUserId(String userId);
}
