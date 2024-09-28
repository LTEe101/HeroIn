package com.ssafy.Heroin.service;

import com.ssafy.Heroin.domain.Title;
import com.ssafy.Heroin.repository.TitleRepository;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@Transactional
@AllArgsConstructor
public class TitleService {

    private final TitleRepository titleRepository;

    public void regist(String title) {
        Title t = new Title();
        t.setTitle(title);
        titleRepository.save(t);
    }
}
