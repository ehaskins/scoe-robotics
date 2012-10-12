#include <Encoder.h>

Encoder enc4(21, 17);
Encoder enc3(20, 16);
Encoder enc2(19, 15);
Encoder enc1(18, 14);

void setup(){
  Serial.begin(9600);
  Serial.println("ready!");
}

long pos1Last;
long pos2Last;
long pos3Last;
long pos4Last;
void loop(){
  long pos1 = enc1.read();
  if (pos1 != pos1Last){
    Serial.print("1:");
    Serial.println(pos1); 
  }
  pos1Last = pos1;  
  
  long pos2 = enc2.read();
  if (pos2 != pos2Last){
    Serial.print("2:");
    Serial.println(pos2); 
  }
  pos2Last = pos2; 
  
  long pos3 = enc3.read();
  if (pos3 != pos3Last){
    Serial.print("3:");
    Serial.println(pos3); 
  }
  pos3Last = pos3; 
  
  long pos4 = enc4.read();
  if (pos4 != pos4Last){
    Serial.print("4:");
    Serial.println(pos4); 
  }
  pos4Last = pos4; 
}

