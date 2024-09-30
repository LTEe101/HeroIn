package com.ssafy.Heroin.repository;

import com.ssafy.Heroin.domain.Title;
import org.springframework.data.jpa.repository.JpaRepository;

public interface TitleRepository extends JpaRepository<Title, Long> {
}
