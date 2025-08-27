using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isoSim1 : AerodynamicCalculator
{
    public override void FlightModelStart(){
        Debug.Log("isoSim1");
        // Set take-off speed
        if(MyGameManeger.instance.FlightMode=="BirdmanRally"){
            //MyGameManeger.instance.Airspeed_TO = 5.0f; // Airspeed at take-off [m/s]
            PlaneRigidbody.velocity = Vector3.zero;
        }else if(MyGameManeger.instance.FlightMode=="TestFlight"){ //
            PlaneRigidbody.velocity = new Vector3(
                Airspeed0*Mathf.Cos(Mathf.Deg2Rad*alpha0)*Mathf.Cos(Mathf.Deg2Rad*MyGameManeger.instance.StartRotation),
                -Airspeed0*Mathf.Sin(Mathf.Deg2Rad*alpha0),
                -Airspeed0*Mathf.Cos(Mathf.Deg2Rad*alpha0)*Mathf.Sin(Mathf.Deg2Rad*MyGameManeger.instance.StartRotation)
            );
        }

        // Calculate CL at cluise
        CL0 = (PlaneRigidbody.mass*Physics.gravity.magnitude)/(0.5f*rho*Airspeed0*Airspeed0*Sw);
        CLt0 = (Cmw0+CL0*hw)/(VH+(St/Sw)*hw);
        CLw0 = CL0-(St/Sw)*CLt0;
        if(Downwash){epsilon0 = (CL0/(Mathf.PI*ew*AR))*Mathf.Rad2Deg;}

        dh0 = Screen.height/2f; // Initial Mouse Position

        //Debug.Log(CLw0);
        hw0 =hw;
        lt0 =lt;
    }
    
    public override void FlightModelFixedUpdate(){
        //入力系統
        //リジットボディに代入
        PlaneRigidbody.centerOfMass = new Vector3(pitchGravity,PlaneRigidbody.centerOfMass.y,PlaneRigidbody.centerOfMass.z);

        //hwに代入する重心位置(%MAC)を計算
        hw2= hw0-(pitchGravity/cMAC);
        //hwに代入
        hw = hw2;

        lt = lt0 + pitchGravity;

        if(MyGameManeger.instance.PlaneName == "Tatsumi"){
            float Iyy = (85.6f*pitchGravity*pitchGravity)+(38.63f*pitchGravity)+1241.85f;
            Vector3 tensor = PlaneRigidbody.inertiaTensor;
            tensor.y = Iyy;
            PlaneRigidbody.inertiaTensor = tensor;
        }

        // Velocity and AngularVelocity
        float u = transform.InverseTransformDirection(PlaneRigidbody.velocity).x;
        float v = -transform.InverseTransformDirection(PlaneRigidbody.velocity).z;
        float w = -transform.InverseTransformDirection(PlaneRigidbody.velocity).y;
        float p = -transform.InverseTransformDirection(PlaneRigidbody.angularVelocity).x*Mathf.Rad2Deg;
        float q = transform.InverseTransformDirection(PlaneRigidbody.angularVelocity).z*Mathf.Rad2Deg;
        float r = transform.InverseTransformDirection(PlaneRigidbody.angularVelocity).y*Mathf.Rad2Deg;
        float hE = PlaneRigidbody.position.y;
        float Distance = (PlaneRigidbody.position-MyGameManeger.instance.PlatformPosition).magnitude-10f;

        // Force and Momentum
        Vector3 AerodynamicForce = Vector3.zero;
        Vector3 AerodynamicMomentum = Vector3.zero;
        Vector3 TakeoffForce = Vector3.zero;

        // Hoerner and Borst (Modified)
        CGE = (CGEMIN+33f*Mathf.Pow((hE/bw),1.5f))/(1f+33f*Mathf.Pow((hE/bw),1.5f));
        if(MyGameManeger.instance.FlightMode=="BirdmanRally" && Distance<-0.5f){
            //CGE = (CGEMIN+33f*Mathf.Pow((hE/bw),1.5f))/(1f+33f*Mathf.Pow((hE/bw),1.5f));
            CGE = (CGEMIN+33f*Mathf.Pow((1.5f/bw),1.5f))/(1f+33f*Mathf.Pow((1.5f/bw),1.5f));
        }
        //Debug.Log(CGE);
        //if (MyGameManeger.instance.MousePitchControl){
        //    dh = -(Input.mousePosition.y-dh0)*0.0002f*MyGameManeger.instance.MouseSensitivity;
        //}

        // Gust
        LocalGustMag = (MyGameManeger.instance.GustMag + MyGameManeger.instance.GustRandValue)*Mathf.Pow((hE/hE0),1f/7f);
        Gust = Quaternion.AngleAxis(MyGameManeger.instance.GustDirection,Vector3.up)*(Vector3.right*LocalGustMag);
        Vector3 LocalGust = this.transform.InverseTransformDirection(Gust);
        float ug = LocalGust.x + 1e-10f;
        float vg = -LocalGust.z;
        float wg = -LocalGust.y;
        if(ug>0){LocalGustDirection = Mathf.Atan(vg/(ug+1e-10f))*Mathf.Rad2Deg;}
        else{LocalGustDirection = Mathf.Atan(vg/(ug+1e-10f))*Mathf.Rad2Deg+vg/Mathf.Abs((vg+1e-10f))*180;}

        // Calculate angles
        Airspeed =    Mathf.Sqrt((u+ug)*(u+ug) + (v+vg)*(v+vg)+(w+wg)*(w+wg));
        Groundspeed = Mathf.Sqrt(u*u + v*v);
        if(SensorPoint != null){
            ALT = SensorPoint.transform.position.y;
        }
        //Debug.Log(Groundspeed);
        alpha = Mathf.Atan((w+wg)/(u+ug))*Mathf.Rad2Deg;
        //Debug.Log(alpha);
        
        beta = Mathf.Atan((v+vg)/Airspeed)*Mathf.Rad2Deg;

        // Wing and Tail
        CLw = CLw0+aw*(alpha-alpha0);
        CLt = CLt0+at*((alpha+MyGameManeger.instance.TailSetDeg-alpha0)+(1f-CGE*(CLw/CLw0))*epsilon0+de*tau+((lt-dh*cMAC)/Airspeed)*q);
        if(Mathf.Abs(CLw)>CLMAX){CLw = (CLw/Mathf.Abs(CLw))*CLMAX;} // Stall
        if(Mathf.Abs(CLt)>CLMAX){CLt = (CLt/Mathf.Abs(CLt))*CLMAX;} // Stall

        // Lift and Drag
        CL = CLw+(St/Sw)*CLt; // CL
        CD = CDp0*(1f+Mathf.Abs(Mathf.Pow((alpha/9f),3f)))+((CL*CL)/(Mathf.PI*ew*AR))*CGE; // CD

        // Force
        Cx = CL*Mathf.Sin(Mathf.Deg2Rad*alpha)-CD*Mathf.Cos(Mathf.Deg2Rad*alpha); // Cx
        Cy = Cyb*beta+Cyp*(1f/Mathf.Rad2Deg)*((p*bw)/(2f*Airspeed))+Cyr*(1f/Mathf.Rad2Deg)*((r*bw)/(2f*Airspeed))+Cydr*dr; // Cy
        Cz = -CL*Mathf.Cos(Mathf.Deg2Rad*alpha)-CD*Mathf.Sin(Mathf.Deg2Rad*alpha); // Cz

        // Torque
        Cl = Clb*beta+Clp*(1f/Mathf.Rad2Deg)*((p*bw)/(2f*Airspeed))+Clr*(1f/Mathf.Rad2Deg)*((r*bw)/(2f*Airspeed))+Cldr*dr; // Cl
        Cm = Cmw0+CLw*hw-VH*CLt+CL*dh; // Cm       
        Cn = Cnb*beta+Cnp*(1f/Mathf.Rad2Deg)*((p*bw)/(2f*Airspeed))+Cnr*(1f/Mathf.Rad2Deg)*((r*bw)/(2f*Airspeed))+Cndr*dr; // Cn

        AerodynamicForce.x = 0.5f*rho*Airspeed*Airspeed*Sw*Cx;
        AerodynamicForce.y = 0.5f*rho*Airspeed*Airspeed*Sw*(-Cz);
        AerodynamicForce.z = 0.5f*rho*Airspeed*Airspeed*Sw*(-Cy);
        //Debug.Log("CLt"+CLt+"CL"+CL+"Cz"+Cz+"z"+AerodynamicForce.y);
        AerodynamicMomentum.x = 0.5f*rho*Airspeed*Airspeed*Sw*bw*(-Cl);//roll
        AerodynamicMomentum.y = 0.5f*rho*Airspeed*Airspeed*Sw*bw*Cn;//yaw
        AerodynamicMomentum.z = 0.5f*rho*Airspeed*Airspeed*Sw*cMAC*Cm;//pitch

        if(MyGameManeger.instance.FlightMode=="BirdmanRally" && Distance<-0.5f){
            
            CalculateRotation();
            
            float W = PlaneRigidbody.mass*Physics.gravity.magnitude;//重力
            float L = 0.5f*rho*Airspeed*Airspeed*Sw*(Cx*Mathf.Sin(Mathf.Deg2Rad*theta)-Cz*Mathf.Cos(Mathf.Deg2Rad*theta));//揚力
            float N = (W-L)*Mathf.Cos(Mathf.Deg2Rad*3.5f); // N=(W-L)*cos(3.5deg)//翼持ちの抵抗力
            float P = (PlaneRigidbody.mass*MyGameManeger.instance.Airspeed_TO*MyGameManeger.instance.Airspeed_TO)/(2f*10f); // P=m*Vto*Vto/2*L//推進力
            
            //離陸方向をYaw回転に合わせて水平方向に修正
            //Vector3 takeoffDirection = Quaternion.Euler(0f, MyGameManeger.instance.StartRotation, 0f) * Vector3.forward;
            //TakeoffForce = takeoffDirection * P;

            //TakeoffForce.y = N*Mathf.Cos(Mathf.Deg2Rad*3.5f);

            //float TOFh = P;
            //float TOFv = N*Mathf.Cos(Mathf.Deg2Rad*3.5f);
            //TakeoffForce.x = TOFv*Mathf.Sin(MyGameManeger.instance.TailRotation) + TOFh*Mathf.Cos(MyGameManeger.instance.TailRotation);
            //TakeoffForce.y = TOFv*Mathf.Cos(MyGameManeger.instance.TailRotation) - TOFh*Mathf.Sin(MyGameManeger.instance.TailRotation);
            //Debug.Log("Power:"+P);
            
            TakeoffForce.x = P*Mathf.Cos(Mathf.Deg2Rad*MyGameManeger.instance.StartRotation);
            TakeoffForce.y = N*Mathf.Cos(Mathf.Deg2Rad*3.5f);
            TakeoffForce.z = -P*Mathf.Sin(Mathf.Deg2Rad*MyGameManeger.instance.StartRotation);
            
            AerodynamicForce.z = 0f;
            AerodynamicMomentum.x = 0f;//
            AerodynamicMomentum.y = 0f;

            //transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, MyGameManeger.instance.TailRotation);
            //PlaneRigidbody.constraints = RigidbodyConstraints.FreezePositionZ;

            if(AerodynamicMomentum.x <= 0){//左から右に吹く風 左翼がより大きな揚力を生む
                if(Mathf.Abs(AerodynamicMomentum.x) > YL*YMin){//左翼が翼持ちの手を離れている状態
                    //Debug.Log("A1");
                    YlMoment = 0;//既に翼持ちを離れている為、翼持ちはモーメントを与えられない
                }
                else{
                    //Debug.Log("B1");
                    YlMoment = -YL*YMin - AerodynamicMomentum.x;//翼持ちが与えるモーメントは、機体を支える最小限のモーメントから風が与えるそれを引いた値である
                }

                if(Mathf.Abs(AerodynamicMomentum.x + YlMoment) <= YL*YrMax){//右翼持ちにまだ余裕がある状態
                    //Debug.Log("C1");
                    YrMoment = -(AerodynamicMomentum.x + YlMoment);//翼持ちに風と逆の翼持ちのモーメントを足した大きな負荷が掛かるが、まだ耐えられる
                }else{
                    //Debug.Log("D1");
                    YrMoment = YL*YrMax;//つり合いが取れずに右翼持ちのモーメントが足りない状態
                }

            }else{//右から左に吹く風 右翼がより大きな揚力を生む
                if(Mathf.Abs(AerodynamicMomentum.x) > YL*YMin){//右翼が翼持ちの手を離れている状態
                    //Debug.Log("A2");
                    YrMoment = 0;
                }
                else{
                    //Debug.Log("B2");
                    YrMoment = YL*YMin - AerodynamicMomentum.x;
                }

                if(Mathf.Abs(AerodynamicMomentum.x + YrMoment) <= YL*YlMax){//左翼持ちにまだ余裕がある状態
                    //Debug.Log("C2");
                    YlMoment = AerodynamicMomentum.x + YrMoment;
                }else{
                    //Debug.Log("D2");
                    YlMoment = YL*YlMax;
                }
                
            }
            //Debug.Log("YlMoment:"+YlMoment+"YrMoment:"+YrMoment+"aeroX:"+AerodynamicMomentum.x);
            //AerodynamicMomentum.x += YrMoment + YlMoment;//最終的なロールモーメントの計算//一旦消す
            MyGameManeger.instance.TakeOff = false;
        }
        else{
            MyGameManeger.instance.TakeOff = true;
            //PlaneRigidbody.constraints = RigidbodyConstraints.None;
        }
        //else if(MyGameManeger.instance.FlightMode=="BirdmanRally" && !AddTaleForce){
        //    AddTaleForce =true;
        //}
        //Debug.Log(AerodynamicForce.z);
        PlaneRigidbody.AddRelativeForce(AerodynamicForce, ForceMode.Force);
        PlaneRigidbody.AddRelativeTorque(AerodynamicMomentum, ForceMode.Force);
        PlaneRigidbody.AddForce(TakeoffForce, ForceMode.Force);
        nz = AerodynamicForce.y/(PlaneRigidbody.mass*Physics.gravity.magnitude);

    }

    void CalculateRotation()
    {
        float q1 = MyGameManeger.instance.Plane.transform.rotation.x;
        float q2 = -MyGameManeger.instance.Plane.transform.rotation.y;
        float q3 = -MyGameManeger.instance.Plane.transform.rotation.z;
        float q4 = MyGameManeger.instance.Plane.transform.rotation.w;
        float C11 = q1*q1-q2*q2-q3*q3+q4*q4;
        float C22 = -q1*q1+q2*q2-q3*q3+q4*q4;
        float C12 = 2f*(q1*q2+q3*q4);
        float C13 = 2f*(q1*q3-q2*q4);
        float C32 = 2f*(q2*q3-q1*q4);

        phi = -Mathf.Atan(-C32/C22)*Mathf.Rad2Deg;
        theta = -Mathf.Asin(C12)*Mathf.Rad2Deg; 
        psi = -Mathf.Atan(-C13/C11)*Mathf.Rad2Deg;
    }

}