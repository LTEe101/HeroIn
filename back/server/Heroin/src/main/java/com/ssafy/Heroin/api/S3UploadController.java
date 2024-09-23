package com.ssafy.Heroin.api;

import com.ssafy.Heroin.service.S3UploadService;
import lombok.AllArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

@RestController
@AllArgsConstructor
@RequestMapping("/api/img")
public class S3UploadController {

    private final S3UploadService s3UploadService;

    @PostMapping("/upload/{var}")
    public ResponseEntity<?> s3Upload(@RequestPart(value = "image", required = false) MultipartFile image, @PathVariable int var) {
        String profileImage = s3UploadService.upload(image, var);
        return ResponseEntity.ok(profileImage);
    }
}
