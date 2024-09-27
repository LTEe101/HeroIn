package com.ssafy.Heroin.service;

import com.ssafy.Heroin.domain.HistoryCard;
import com.ssafy.Heroin.dto.historycard.RegistHistoryCardDto;
import com.ssafy.Heroin.repository.HistoryCardRepository;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.multipart.MultipartFile;

@Service
@Transactional
@AllArgsConstructor
public class HistoryCardService {

    private final HistoryCardRepository historyCardRepository;
    private final S3UploadService s3UploadService;

    public void historyCardRegist(MultipartFile historyCard, RegistHistoryCardDto registHistoryCardDto) {
        String cardImg = s3UploadService.upload(historyCard);

        HistoryCard historyCardEntity = new HistoryCard();
        historyCardEntity.setDescription(registHistoryCardDto.getDescription());
        historyCardEntity.setName(registHistoryCardDto.getCardName());
        historyCardEntity.setImgNo(cardImg);

        historyCardRepository.save(historyCardEntity);
    }

}
