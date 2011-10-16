################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
../FrcComms/BitField16.cpp \
../FrcComms/BitField8.cpp \
../FrcComms/ByteReader.cpp \
../FrcComms/ByteWriter.cpp \
../FrcComms/CRC32.cpp \
../FrcComms/Configuration.cpp \
../FrcComms/ControlData.cpp \
../FrcComms/FRCCommunication.cpp \
../FrcComms/Joystick.cpp \
../FrcComms/Mode.cpp \
../FrcComms/RobotStatusLight.cpp \
../FrcComms/StatusData.cpp \
../FrcComms/cpp.cpp 

OBJS += \
./FrcComms/BitField16.o \
./FrcComms/BitField8.o \
./FrcComms/ByteReader.o \
./FrcComms/ByteWriter.o \
./FrcComms/CRC32.o \
./FrcComms/Configuration.o \
./FrcComms/ControlData.o \
./FrcComms/FRCCommunication.o \
./FrcComms/Joystick.o \
./FrcComms/Mode.o \
./FrcComms/RobotStatusLight.o \
./FrcComms/StatusData.o \
./FrcComms/cpp.o 

CPP_DEPS += \
./FrcComms/BitField16.d \
./FrcComms/BitField8.d \
./FrcComms/ByteReader.d \
./FrcComms/ByteWriter.d \
./FrcComms/CRC32.d \
./FrcComms/Configuration.d \
./FrcComms/ControlData.d \
./FrcComms/FRCCommunication.d \
./FrcComms/Joystick.d \
./FrcComms/Mode.d \
./FrcComms/RobotStatusLight.d \
./FrcComms/StatusData.d \
./FrcComms/cpp.d 


# Each subdirectory must supply rules for building sources it contributes
FrcComms/%.o: ../FrcComms/%.cpp
	@echo 'Building file: $<'
	@echo 'Invoking: AVR C++ Compiler'
	avr-g++ -I"C:\Users\ehaskins\Code Local\CARD - 2011\Robot\Arduino\Arduino" -Wall -Os -fpack-struct -fshort-enums -ffunction-sections -fdata-sections -Wl,--gc-sections -funsigned-char -funsigned-bitfields -fno-exceptions -mmcu=atmega2560 -DF_CPU=16000000UL -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -c -o"$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


