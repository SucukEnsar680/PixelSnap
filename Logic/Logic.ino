#define CAMERA_MODEL_XIAO_ESP32S3 // Has PSRAM
#include <Arduino.h>
#include "esp_camera.h"
#include "camera_pins.h"
camera_fb_t *capturedImage = NULL;


#define BUTTON_PIN 9  // GPIO9 für den Taster
struct Color {
    uint8_t r, g, b;
};

void setup() {
    pinMode(BUTTON_PIN, INPUT_PULLUP); // Knopf mit internem Pull-up aktivieren
    Serial.begin(115200);
    camera_config_t config;
    config.ledc_channel = LEDC_CHANNEL_0;
    config.ledc_timer = LEDC_TIMER_0;
    config.pin_d0 = GPIO_NUM_0;
    config.pin_d1 = GPIO_NUM_1;
    config.pin_d2 = GPIO_NUM_2;
    config.pin_d3 = GPIO_NUM_3;
    config.pin_d4 = GPIO_NUM_4;
    config.pin_d5 = GPIO_NUM_5;
    config.pin_d6 = GPIO_NUM_6;
    config.pin_d7 = GPIO_NUM_7;
    config.pin_xclk = GPIO_NUM_8;
    config.pin_pclk = GPIO_NUM_9;
    config.pin_vsync = GPIO_NUM_10;
    config.pin_href = GPIO_NUM_11;
    config.pin_sscb_sda = GPIO_NUM_12;
    config.pin_sscb_scl = GPIO_NUM_13;
    config.pin_pwdn = GPIO_NUM_14;
    config.pin_reset = GPIO_NUM_15;

    config.xclk_freq_hz = 20000000;
    config.pixel_format = PIXFORMAT_JPEG;
    config.frame_size = FRAMESIZE_VGA;
    config.jpeg_quality = 12;
    config.fb_count = 1;



    esp_err_t err = esp_camera_init(&config);
    if (err != ESP_OK) {
      Serial.println("Kamera konnte nicht initialisiert werden!");
      Serial.println(esp_err_to_name(err));  // Gibt den Fehlercode als String aus

      return;
    }
    Serial.println("Kamera erfolgreich initialisiert");
}

void loop() {
    if (digitalRead(BUTTON_PIN) == LOW) { // Knopf gedrückt
        Serial.println("Knopf wurde gedrückt!");
        TakePicture();

        delay(200);  // Prellen verhindern
    }
}


void TakePicture(){
  capturedImage = esp_camera_fb_get();
    if (!capturedImage) {
        Serial.println("Fehler beim Aufnehmen des Bildes!");
    } else {
        Serial.println("Bild erfolgreich aufgenommen!");
        Serial.printf("Bildgröße: %d Bytes\n", capturedImage->len);
        int pixelCount = capturedImage->len / 2;  // RGB565 hat 2 Byte pro Pixel
        Color imageData[pixelCount];
        for (int i = 0; i < pixelCount; i++) {
          uint16_t pixel = ((uint16_t*)capturedImage->buf)[i];
          imageData[i].r = ((pixel >> 11) & 0x1F) << 3;  // R 5-bit -> 8-bit
          imageData[i].g = ((pixel >> 5) & 0x3F) << 2;   // G 6-bit -> 8-bit
          imageData[i].b = (pixel & 0x1F) << 3;          // B 5-bit -> 8-bit
        
        }
        int numColors = 16;
        applyKMeans(imageData, pixelCount, numColors);


      }
    
}






// KMEANS



// Funktion zur Berechnung der euklidischen Distanz zwischen zwei Farben
int colorDistance(Color c1, Color c2) {
    return (c1.r - c2.r) * (c1.r - c2.r) + 
           (c1.g - c2.g) * (c1.g - c2.g) + 
           (c1.b - c2.b) * (c1.b - c2.b);
}

void applyKMeans(Color *imageData, int pixelCount, int numColors) {
    Color centroids[numColors];        // Cluster-Mittelpunkte
    Color newCentroids[numColors];     // Neue Mittelpunkte
    int counts[numColors] = {0};       // Anzahl der zugewiesenen Pixel pro Cluster

    // Initialisierung der Cluster mit zufälligen Farben aus dem Bild
    for (int i = 0; i < numColors; i++) {
        centroids[i] = imageData[random(pixelCount)];
    }

    // K-Means Iteration
    for (int iteration = 0; iteration < 10; iteration++) {  // 10 Iterationen
        memset(newCentroids, 0, sizeof(newCentroids));
        memset(counts, 0, sizeof(counts));

        // Zuweisung der Pixel zu Clustern
        for (int i = 0; i < pixelCount; i++) {
            int bestCluster = 0;
            int bestDist = colorDistance(imageData[i], centroids[0]);

            for (int j = 1; j < numColors; j++) {
                int dist = colorDistance(imageData[i], centroids[j]);
                if (dist < bestDist) {
                    bestCluster = j;
                    bestDist = dist;
                }
            }

            // Neue Cluster-Mittelpunkte berechnen
            newCentroids[bestCluster].r += imageData[i].r;
            newCentroids[bestCluster].g += imageData[i].g;
            newCentroids[bestCluster].b += imageData[i].b;
            counts[bestCluster]++;
        }

        // Aktualisierung der Cluster-Mittelpunkte
        for (int i = 0; i < numColors; i++) {
            if (counts[i] > 0) {
                centroids[i].r /= counts[i];
                centroids[i].g /= counts[i];
                centroids[i].b /= counts[i];
            }
        }
    }

    // Anwendung der neuen Farben auf das Bild
    for (int i = 0; i < pixelCount; i++) {
        int bestCluster = 0;
        int bestDist = colorDistance(imageData[i], centroids[0]);

        for (int j = 1; j < numColors; j++) {
            int dist = colorDistance(imageData[i], centroids[j]);
            if (dist < bestDist) {
                bestCluster = j;
                bestDist = dist;
            }
        }
        imageData[i] = centroids[bestCluster];  // Setzt Pixel auf Clusterfarbe
    }
}