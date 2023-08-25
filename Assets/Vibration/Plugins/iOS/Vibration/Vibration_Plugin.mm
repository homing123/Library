//
//  Vibration_Plugin.cpp
//  Unity-iPhone
//
//  Created by Sususoft on 2023/04/11.
//

#include "Vibration_Plugin.h"
#include <AudioToolbox/AudioToolbox.h>

@implementation Vibration_Plugin

extern "C" {
    void vibrateWithIdx(int idx)
    {
        switch(idx){
            case 0:
                AudioServicesPlaySystemSound(1519);
                break;
            case 1:
                AudioServicesPlaySystemSound(1520);
                break;
            case 2:
                AudioServicesPlaySystemSoundWithCompletion(kSystemSoundID_Vibrate, ^{
                    // 진동이 끝나면 실행될 콜백 함수
                });
                break;
        }
    }
    
}
@end
