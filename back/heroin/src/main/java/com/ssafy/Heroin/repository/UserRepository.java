package com.ssafy.Heroin.repository;

import com.ssafy.Heroin.domain.User;
import org.springframework.data.repository.CrudRepository;

import java.util.Optional;

public interface UserRepository extends CrudRepository<User, Integer> {

    Optional<User> findByUserId(String userId);
    boolean existsByUserId(String userId);
}
