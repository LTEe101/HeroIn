package com.ssafy.Heroin.domain;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.annotations.CreationTimestamp;

import java.time.LocalDateTime;
import java.util.List;

@Getter
@Setter
@Entity
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "usertable_id")
    private Long id;

    private String username;

    private String userId;

    private String userPw;

    private String imgNo;

    private String title;

    @CreationTimestamp
    private LocalDateTime createAt;

    @OneToMany(mappedBy = "user")
    private List<UserHistoryCard> historyCards;

}
