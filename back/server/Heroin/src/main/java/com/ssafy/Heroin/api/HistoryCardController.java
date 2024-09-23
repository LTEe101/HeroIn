package com.ssafy.Heroin.api;

import com.ssafy.Heroin.dto.historycard.RegistHistoryCardDto;
import com.ssafy.Heroin.service.HistoryCardService;
import lombok.AllArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

@RestController
@AllArgsConstructor
@RequestMapping("/api/history")
public class HistoryCardController {

    private final HistoryCardService historyCardService;

    @PostMapping("/regist")
    public ResponseEntity<?> registCard(
            @RequestPart(value = "cardName", required = false) MultipartFile historyCard,
            @RequestPart(value = "HistoryCardDto", required = false) RegistHistoryCardDto registHistoryCardDto)
    {

        historyCardService.historyCardRegist(historyCard, registHistoryCardDto);

        return ResponseEntity.ok().build();
    }
}
