// ///////////////////////// Bibliotecas ///////////////////////// //
#include <WiFi.h>
#include <WiFiUdp.h>
#include <Wire.h>

// ///////////////////////// Definición MD25 ///////////////////////// //
#define CMD             (byte)0x00
#define MD25ADDRESS     0x58
#define SOFTWAREREG     0x0D
#define SPEED1          (byte)0x00
#define SPEED2          0x01
#define ENCODERONE      0x02
#define ENCODERTWO      0x06
#define VOLTREAD        0x0A
#define RESETENCODERS   0x20

// ///////////////////////// Red local ///////////////////////// //
#ifndef STASSID
#define STASSID "MiRedWifi"
#define STAPSK "123456789"
#endif

// ///////////////////////// Protocolo UDP ///////////////////////// //
#define UDP_TX_PACKET_MAX_SIZE 512 // Tamaño máximo del paquete UDP
unsigned int localPort = 58625;  // Puerto local para escuchar
char packetBuffer[UDP_TX_PACKET_MAX_SIZE + 1];  // Búfer para contener el paquete entrante
WiFiUDP Udp;     // Instancia UDP

// ///////////////////////// Comunicación I2C ///////////////////////// //
#define I2C_SDA 14
#define I2C_SCL 15

void setup() {
  Serial.begin(115200);
  
  WiFi.mode(WIFI_STA);
  WiFi.begin(STASSID, STAPSK);
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print('.');
    delay(500);
  }
  Serial.print("¡Conectado! Dirección IP: ");
  Serial.println(WiFi.localIP());
  Serial.printf("Servidor UDP en el puerto %d\n", localPort);
  Udp.begin(localPort);

  // Configuración de los pines SDA y SCL para la comunicación I2C
  Wire.begin(I2C_SDA, I2C_SCL, 100000);
  Serial.printf("Conexión I2C SDA: %d    SCL: %d\n", I2C_SDA, I2C_SCL);
}

void loop() {
  // Si hay datos disponibles, lee un paquete
  int packetSize = Udp.parsePacket();
  if (packetSize) {
    Serial.printf("Recibido paquete de tamaño %d de %s:%d\n    (a %s:%d, espacio libre = %d B)\n", packetSize, Udp.remoteIP().toString().c_str(), Udp.remotePort(), Udp.remoteIP().toString().c_str(), Udp.remotePort(), ESP.getFreeHeap());
    // Lectura del paquete en packetBufffer
    int n = Udp.read(packetBuffer, UDP_TX_PACKET_MAX_SIZE);
    
    if(n > 0){
      ////// Muestra de los datos recibidos por terminal serial
      packetBuffer[n] = 0;
      Serial.println("Contenido:");
      int a = packetBuffer[0];
      int b = packetBuffer[1];
      Serial.print("a: ");
      Serial.print(a);
      Serial.print("    b: ");
      Serial.println(b);

      ////// Envío de la información por I2C a la trajeta MD25
      // Motor derecho
      Wire.beginTransmission(MD25ADDRESS);
      Wire.write(SPEED2);
      Wire.write(a);
      Wire.endTransmission();
      // Motor izquierdo
      Wire.beginTransmission(MD25ADDRESS);
      Wire.write(SPEED1);
      Wire.write(b);
      Wire.endTransmission();
  
      //////// Respuesta por protocolo UDP a Matlab
      Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
      Udp.write((const uint8_t *)&a, 1); // Enviar velocidad a
      Udp.write((const uint8_t *)&b, 1); // Enviar velocidad b
      Udp.endPacket();
    }
  }
}
