using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHelper : MonoBehaviour
{
    GameHelper _gameHelper;
    Transform _player;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while(_gameHelper == null)
        {
            _gameHelper = GameObject.FindObjectOfType<GameHelper>();
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameHelper == null ||
            (_gameHelper != null && _gameHelper.CurrentPlayer == null))
            return;
        _player = _gameHelper.CurrentPlayer.transform;
        Vector3 newPosition = new Vector3(_player.position.x, _player.position.y, transform.position.z);
        transform.position = newPosition;
        //transform.position = Vector3.MoveTowards(transform.position,newPosition,Time.deltaTime * 15);
        Debug.Log("CameraChangePos");
    }
}
