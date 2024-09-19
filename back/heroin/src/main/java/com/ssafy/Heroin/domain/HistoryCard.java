package com.ssafy.Heroin.domain;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

import java.util.List;

@Getter
@Setter
@Entity
public class HistoryCard {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "history_id")
    private Long id;

    @OneToMany
    private List<UserHistoryCard> userHistoryCards;

    private String name;

    private String description;

    private String imgNo;
}
