#include "BalBot.h"
#include <Arduino.h>
#include <Servo\Servo.h>
#include <Encoder\Encoder.h>
#include "utils.h"
#include "PID.h"
#include "Gyro.h"
#include "Accelerometer.h"
#include "SimpleAngleThing.h"
// 0 gyroX - tilt
// 1 gyroY - spin
// 2 gyroZ
// 3 accelX - up
// 4 accelY
// 5 accelZ - forward

#define BALANCE_SAFTEY_TILT 10

#define LEFT_MOTOR 3
#define RIGHT_MOTOR 2
#define RIGHT_INVERT 1
#define LEFT_INVERT -1

Gyro TiltGyro(0, -0.7);
Accelerometer UpAccel(3, 512, false);
Accelerometer ForwardAccel(5, 512, true);
SimpleAngleThing AngleCalc(&TiltGyro, &ForwardAccel, &UpAccel, 0.98, true);

Encoder leftEnc (21, 18);
Encoder rightEnc (20,17);

Servo left;
Servo right;

#define PULSE_RANGE 750
#define LEFT_CENTER 1500
#define RIGHT_CENTER 1490
#define PLUSE_DEADBAND 80

//PID BalancePID(-40, -2, -25);
PID BalancePID(-40, 0, 0);

void setup() {
	pinMode(13, OUTPUT);
	writeLed(true);
	Serial.begin(115200);
	
	calibrate(2000, 200);
	
	right.attach(RIGHT_MOTOR);
	left.attach(LEFT_MOTOR);

	Serial.println("Ready.");
	writeLed(false);
}

unsigned long lastLoop = 0;
double elapsedSeconds = 0.0;
void loop() {
	long nowMicros = micros();
	elapsedSeconds = (double)(nowMicros - lastLoop) / 1000000;
	
	if (elapsedSeconds > 0.01){
		lastLoop = nowMicros;
		balance(-1, 0);
		//printSensors();
		//printImuCsv();
		//testCenter();
	}
}

void writeLed(bool state){
	digitalWrite(13, state ? HIGH : LOW);
}

void balance(double desiredAngle, double spin){
	AngleCalc.update();
	double output = 0.0;
	double error = desiredAngle - AngleCalc.angle;
	
	if (abs(error) > BALANCE_SAFTEY_TILT){
		output = 0;
		BalancePID.iTotal = 0.0;
		spin = 0;
	}
	else{
		output = BalancePID.update(AngleCalc.angle, desiredAngle);
	}
	setDrive(output + spin, output - spin);
}

void setDrive(int leftVal, int rightVal){
	left.writeMicroseconds(LEFT_CENTER + removeDeadband(leftVal * LEFT_INVERT, PLUSE_DEADBAND, PULSE_RANGE));
	right.writeMicroseconds(RIGHT_CENTER + removeDeadband(rightVal * RIGHT_INVERT, PLUSE_DEADBAND, PULSE_RANGE));
}

void calibrate(int calibrationDelay, int calibrationLoops) {
	Serial.println("Waiting to calibrate...");
	delay(calibrationDelay);
	Serial.println("Calibrating...");

	for (int i = 0; i < calibrationLoops; i++) {
		TiltGyro.calibrate();
		delay(5);
	}
	
	TiltGyro.endCalibrate();

	Serial.println("Calibration complete!");
	Serial.print("GyroCenter:");
	Serial.print(TiltGyro.center);
}