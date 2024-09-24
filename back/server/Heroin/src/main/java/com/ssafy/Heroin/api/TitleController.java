package com.ssafy.Heroin.api;

import com.ssafy.Heroin.service.TitleService;
import lombok.AllArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@AllArgsConstructor
@RequestMapping("/api/title")
public class TitleController {

    private final TitleService titleService;

    @PostMapping("/regist")
    public ResponseEntity<?> addTitle(@RequestParam String title) {
        titleService.regist(title);

        return ResponseEntity.ok().build();
    }
}
