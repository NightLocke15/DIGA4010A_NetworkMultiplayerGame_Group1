using Mirror;
using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : NetworkBehaviour
{
    [SyncVar]
    public bool shake;
    [SyncVar]
    public float shakeTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    [ClientCallback]
    void Update()
    {
       if (shake == true)
       {
           shakeTime += Time.deltaTime;

            CameraShakeCmd();

       }

       if (shakeTime > 0.2f)
       {
           shake = false;
           shakeTime = 0;
            CameraStopCmd();


       }
    }

    [ClientCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Puck" || collision.collider.tag == "Enemy")
        {
            shake = true;
        }
    }

    [Command(requiresAuthority = false)] 
    public void CameraShakeCmd()
    {
        CameraShakeRpc();
    }

    [ClientRpc]
    public void CameraShakeRpc()
    {
        if (isServer)
        {
            GameObject.Find("CinemachineCameraOne").GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 1;
            GameObject.Find("CinemachineCameraOne").GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = 1;
        }
        else
        {
            GameObject.Find("CinemachineCameraTwo").GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 1;
            GameObject.Find("CinemachineCameraTwo").GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = 1;
        }
    }

    [Command(requiresAuthority = false)]
    public void CameraStopCmd()
    {
        CameraStopRpc();
    }

    [ClientRpc]
    public void CameraStopRpc()
    {
        if (isServer)
        {
            GameObject.Find("CinemachineCameraOne").GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0;
            GameObject.Find("CinemachineCameraOne").GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = 0;
        }
        else
        {
            GameObject.Find("CinemachineCameraTwo").GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0;
            GameObject.Find("CinemachineCameraTwo").GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = 0;
        }
    }

    
}
