using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace IPS.Inputs
{
    public class NPC : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField] GameObject body;
        GameObject shotScoreText;
        public void Shot(Transform sniperShotBy)
        {

            Debug.Log("NPC has been shot!");
            EffectPool.Instance.GetPooledObject("Effect_CloudBurst", gameObject.transform.position);
            shotScoreText = EffectPool.Instance.GetPooledObject("ScoreText", gameObject.transform.position, true);
            shotScoreText.GetComponent<TextMeshPro>().text = "-" + ScoreManagerIPS.Instance.pointsLossForNPCShot;
            shotScoreText.transform.LookAt(sniperShotBy); //text face camera of player who shot
            shotScoreText.transform.Rotate(0f,180f,0f,Space.Self);
            gameObject.GetComponent<Animator>().SetTrigger("Shot");
        }

        public void Die()
        { //activated by animator
            EffectPool.Instance.GetPooledObject("Effect_DeathCloud", body.transform.position, true);
            shotScoreText.SetActive(false);
            
            Destroy(this.gameObject);
        }
    }
}
