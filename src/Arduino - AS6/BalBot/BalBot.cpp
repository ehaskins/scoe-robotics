#include "BalBot.h"
#include <Arduino.h>
#include <Servo\Servo.h>
#include <Encoder\Encoder.h>
#include "utils.h"
// 0 gyroX - tilt
// 1 gyroY - spin
// 2 gyroZ
// 3 accelX - up
// 4 accelY
// 5 accelZ - forward

#define ANGLE_PRINT_INTERVAL 500
#define TILT_GYRO 0
#define SPIN_GYRO 1

#define ACCEL_UP 3
#define ACCEL_FORWARD 5

#define LEFT_MOTOR 3
#define RIGHT_MOTOR 2
#define RIGHT_INVERT 1
#define LEFT_INVERT -1

#define LEFT_ENC_A 21
#define LEFT_ENC_B 18
#define RIGHT_ENC_A 20
#define RIGHT_ENC_B 17

#define PULSE_RANGE 750
#define LEFT_CENTER 1500
#define RIGHT_CENTER 1490
#define PLUSE_DEADBAND 80

Servo left;
Servo right;

Encoder leftEnc (LEFT_ENC_A, LEFT_ENC_B);
Encoder rightEnc (RIGHT_ENC_A,RIGHT_ENC_B);

void setup() {
	pinMode(13, OUTPUT);
	writeLed(true);
	Serial.begin(115200);

	right.attach(RIGHT_MOTOR);
	left.attach(LEFT_MOTOR);
	
	calibrate(2000, 200);
	
	Serial.println("Ready.");
	writeLed(false);
}


int amt = 0;
int dir = 1;
int minPos = -20;
int maxPos = 20;
unsigned long startMillis = 0;

int count = 0;
unsigned long lastLoop = 0;
double elapsedSeconds = 0.0;
void loop() {
	long nowMicros = micros();
	elapsedSeconds = (double)(nowMicros - lastLoop) / 1000000;
	
	if (elapsedSeconds > 0.01){
		lastLoop = nowMicros;
		updateCurrentAngle();
		balance(0);
		//printSensors();
		//printImuCsv();
		//testCenter();
	}
}

void writeLed(bool state){
	digitalWrite(13, state ? HIGH : LOW);
}

double angle = 0;
double rate = 0;
double spinRate = 0;

#define BAL_P -60
#define BAL_I 0
#define BAL_D 0

#define BALANCE_SAFTEY_TILT 10

double balanceI = 0.0;
double balanceLastError = 0.0;
void balance(double desiredAngle){
	double error = desiredAngle - angle;
	double output = error * BAL_P;
	balanceI += error * BAL_I;
	output += balanceI;
	double errorDiff = balanceLastError - error;
	output += errorDiff * BAL_D;
	
	if (abs(error) > BALANCE_SAFTEY_TILT){
		output = 0;
	}
	setDrive(output, output);
}

void setDrive(int leftVal, int rightVal){
	left.writeMicroseconds(LEFT_CENTER + removeDeadband(leftVal * LEFT_INVERT, PLUSE_DEADBAND, PULSE_RANGE));
	right.writeMicroseconds(RIGHT_CENTER + removeDeadband(rightVal * RIGHT_INVERT, PLUSE_DEADBAND, PULSE_RANGE));
}

int pos = 0;
int range = 200;
int testCenterDir = 1;
void testCenter(){
	pos += testCenterDir;
	if (pos > range || pos < -range)
	testCenterDir *= -1;
	setDrive(pos,pos);
	Serial.println(pos);
}


double gyroDegSecUnit = -0.7;
long accelYUnitsG = 930;
long accelXUnitsG = 930;
long accelCenter = 512;

double gyroWeight = 0.98;
double accelWeight = 0.02;

long gyroCenter = 0;
long spinGyroCenter = 0;

bool firstAngleUpdate = true;
int lastAnglePrint = 0;

void updateCurrentAngle() {
	int gyroRaw = analogRead(TILT_GYRO);
	int accelXRaw = analogRead(ACCEL_FORWARD);
	int accelYRaw = analogRead(ACCEL_UP);
	int spinRaw = analogRead(SPIN_GYRO);

	accelXRaw -= accelCenter;
	accelYRaw -= accelCenter;
	gyroRaw -= gyroCenter;
	spinRaw -= spinGyroCenter;
	
	//gyroRaw *= -1;
	accelXRaw *= -1;
	//accelYRaw *= -1;

	spinRate = (double) spinRaw / gyroDegSecUnit;

	double accelRads = atan2(accelXRaw, accelYRaw);
	double accelAngle = (long) (accelRads * 180 / PI);


	rate = (double) gyroRaw / gyroDegSecUnit;
	double gyroDelta = rate * elapsedSeconds;

	if (firstAngleUpdate) {
		angle = accelAngle;
		firstAngleUpdate = false;
	}

	angle = (angle + gyroDelta) * gyroWeight + accelAngle * accelWeight;
	
	int now = millis();
	if (now - lastAnglePrint > ANGLE_PRINT_INTERVAL){
		lastAnglePrint = now;
		
		Serial.print("Angle : ");
		Serial.print((int)angle);
		Serial.print(" AccelAngle : ");
		Serial.print((int)accelAngle);
		Serial.print(" GyroChange : ");
		Serial.println(gyroDelta);
	}
}

void calibrate(int calibrationDelay, int calibrationLoops) {
	Serial.println("Waiting to calibrate...");
	delay(calibrationDelay);
	Serial.println("Calibrating...");

	long gyroTotal = 0;
	long spinGyroTotal = 0;

	for (int i = 0; i < calibrationLoops; i++) {
		gyroTotal += analogRead(TILT_GYRO);
		spinGyroTotal += analogRead(SPIN_GYRO);

		delay(5);
	}

	gyroCenter = gyroTotal / calibrationLoops;
	spinGyroCenter = spinGyroTotal / calibrationLoops;

	Serial.println("Calibration complete!");
	Serial.print("gyroTotal:");
	Serial.print(gyroTotal);
	Serial.print(" loops:");
	Serial.print(calibrationLoops);
	Serial.print(" gyroCenter:");
	Serial.print(gyroCenter);
	Serial.print(" spinGyroCenter:");
	Serial.println(spinGyroCenter);
}

void printSensors(){
	delay(100);
	
	amt += dir;
	if (amt > maxPos || amt < minPos){
		dir *= -1;
	}
	right.write(90+(amt * RIGHT_INVERT));
	left.write(90+(amt * LEFT_INVERT));
	count++;
	
	if (count % 20 == 0){
		for (int i = 1; i <= 6; i++){
			int value = analogRead(i);
			//Serial.print(i);
			Serial.print(",");
			Serial.println(value);
		}
		
		Serial.print("Left:");
		Serial.print(leftEnc.read() * LEFT_INVERT);
		Serial.print(" Right:");
		Serial.println(rightEnc.read() * RIGHT_INVERT);
		
		Serial.println();
	}
}

void printImuCsv(){
	int last = 0;
	while (count++ < 10000){
		int anas[6];
		for (int i = 0; i < 6; i++){
			anas[i] = analogRead(i);
		}
		
		Serial.print(count);
		Serial.print(",");
		for (int i = 0; i < 6; i++){
			Serial.print(anas[i]);
			Serial.print(",");
		}
		Serial.print((long)millis());
		
		Serial.println();
		//delay(1);
	}
	delay(10000);
}