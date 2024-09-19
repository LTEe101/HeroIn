package com.ssafy.Heroin.service;

import com.ssafy.Heroin.repository.UserHistoryCardRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@Transactional
@RequiredArgsConstructor
public class UserHistoryCardService {

    private final UserHistoryCardRepository userHistoryCardRepository;


}
