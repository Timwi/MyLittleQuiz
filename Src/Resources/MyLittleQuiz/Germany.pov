#version 3.7;

#declare use_arealight = off;
#declare use_radiosity = off;

#include "Generated/Germany.pov"
#include "Generated/Berlin.pov"


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

background { rgb <0, 0, 0>/255 }
// background { rgb 1 } // for print

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

// x: 230..780
// y: -100..-800

mklight(< 200-500, -600, -700>, rgb .3, 100)        
mklight(< 800-500, -840, -999>, rgb .2, 500)       
mklight(< 790-500, -500, -1200>, rgb .3, 1000)      
mklight(<1130-500, -280, -800>, rgb .15, 1000)       

camera {
        right
        x*image_width/image_height
        
        // Germany
        location <400, -450, -800>
        look_at <400, -450, 0>

        // Berlin
        //location <675, -326, -40>
        //look_at <675, -326, 0>
        
}                             

object { 
        Germany
        
        texture {
                pigment { color rgb<10, 255, 85>/255 }
                finish { phong 1 reflection 0.00 }
        }
}
object { 
        Berlin
        
        texture {
                pigment { color rgb<40, 120, 255>/255 }
                finish { phong 1 reflection 0.00 }
        }
        
        translate -.5*z
}    

plane {
        z 0
        texture {
                pigment { color rgb 1 }
        }        
        translate -10000*z
}
