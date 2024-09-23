package com.ssafy.Heroin.repository;

import com.ssafy.Heroin.domain.HistoryCard;
import org.springframework.data.jpa.repository.JpaRepository;

public interface HistoryCardRepository extends JpaRepository<HistoryCard, Long> {
    HistoryCard findById(long id);
}
