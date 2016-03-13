#version 3.7;

#declare use_arealight = on;
#declare use_radiosity = on;

#include "My.pov"
#include "MyHeart.pov"
#include "Little.pov"
#include "Quiz.pov"
#include "MyCloud.pov"
#include "LittleCloud.pov"
#include "QuizCloud.pov"
#include "RainbowCloud.pov"
#include "Rainbow1.pov"
#include "Rainbow2.pov"
#include "Rainbow3.pov"
#include "Rainbow4.pov"
#include "Rainbow5.pov"


global_settings { assumed_gamma 2.2 }
        
#if (use_radiosity)        
        global_settings {
          radiosity {
            pretrace_start 0.08           // start pretrace at this size
            pretrace_end   0.04           // end pretrace at this size
            count 35                      // higher -> higher quality (1..1600) [35]
        
            nearest_count 5               // higher -> higher quality (1..10) [5]
            error_bound 1.8               // higher -> smoother, less accurate [1.8]
            recursion_limit 3             // how much interreflections are calculated (1..5+) [3]
        
            low_error_factor .5           // reduce error_bound during last pretrace step
            gray_threshold 0.0            // increase for weakening colors (0..1) [0]
            minimum_reuse 0.015           // reuse of old radiosity samples [0.015]
            brightness 1                  // brightness of radiosity effects (0..1) [1]
        
            adc_bailout 0.01/2
          }
        }
#end

background { rgb <0, 30, 111>/255 }

#macro mklight(loc, c, sz)
        light_source {
            loc
            color c
            #if (use_arealight)
                area_light <sz, 0, 0> <0, sz, 0> 4, 4
                jitter
            #end    
            photons { refraction on  reflection on }
        }
#end

mklight(<-300, 1500, -700>, rgb .4, 100)        // top-left (near 'my')
mklight(< 300, 2100, -999>, rgb .25, 500)       // top
mklight(< 290, 1240, -1200>, rgb .4, 1000)        // middle
mklight(< 630,  700, -800>, rgb .2, 1000)        // bottom right (on curl)

// x: 0..725
// y: 625..1200         

camera {
        right
        x*image_width/image_height
        location <500, 910, -800>
        look_at <365, 990, 0>
}                             

object { 
        MyHeart
        
        rotate -180*x

        texture {
                pigment { color rgb<0, 204, 71>/255 }
                finish { phong 1 reflection 0.00 }
        }
}
object { 
        My
        
        rotate -180*x

        texture {
                pigment { color rgb<255, 255, 210>/255 }
                finish { phong 1 reflection 0.00 }
        }
}
object { 
        Little 
        
        rotate -180*x

        texture {
                pigment { color rgb<255, 71, 51>/255 }
                finish { phong 1 reflection 0.00 }
        }
}
sphere {        // i-dot in 'LITTLE'
        <342, 1170, -30> 16
        
        texture {
                pigment { color rgb<255, 71, 51>/255 }
                finish { phong 1.0 reflection 0.00 }
        }
}
object { 
        Quiz 
        
        rotate -180*x

        texture {
                pigment { color rgb<0, 137, 250>/255 }
                finish { phong 1.0 reflection 0.00 }
        }
}
sphere {        // i-dot in 'QUIZ'
        <470, 1055, -35> 35
        
        texture {
                pigment { color rgb<61, 181, 255>/255 }
                finish { phong 1.0 reflection 0.00 }
        }
}           

object {
        Rainbow1
        rotate -180*x
        
        texture {
                pigment { color rgb 1 }
                finish { phong 1.0 reflection 0.00 }
        }
}
object {
        Rainbow2
        rotate -180*x
        
        texture {
                pigment { color rgb<246, 183, 183>/255 }
                finish { phong 1.0 reflection 0.00 }
        }
}
object {
        Rainbow3
        rotate -180*x
        
        texture {
                pigment { color rgb<239, 236, 103>/255 }
                finish { phong 1.0 reflection 0.00 }
        }
}
object {
        Rainbow4
        rotate -180*x
        
        texture {
                pigment { color rgb<41, 215, 102>/255 }
                finish { phong 1.0 reflection 0.00 }
        }
}
object {
        Rainbow5
        rotate -180*x
        
        texture {
                pigment { color rgb<37, 78, 153>/255 }
                finish { phong 1.0 reflection 0.00 }
        }
}

object {    
        union {
                object { MyCloud }
                object { LittleCloud }
                object { QuizCloud }  
                object { RainbowCloud }
        }

        rotate -180*x

        texture {
                pigment { color rgb 1 }
                finish { phong 1.0 reflection 0.00 }
        }
}

plane {
        z 0
        texture {
                pigment { color rgb 1 }
        }        
        translate -10000*z
}
