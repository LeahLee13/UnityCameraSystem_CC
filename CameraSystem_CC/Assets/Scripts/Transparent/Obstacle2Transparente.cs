using UnityEngine;
using System.Collections.Generic;

public class Obstacle2Transparente : MonoBehaviour
{
    private GameObject player;
    public Material alphaMaterial;//透明材质

    private List<RaycastHit> hits;
    private List<HitInfo> changedInfos = new List<HitInfo>();

    private struct HitInfo
    {
        public GameObject obj;
        public Renderer[] renderers;
        public List<Material> materials;
    }

    void Update()
    {
        if (player == null)
        {
            player = GameManager.Instance.playerObj;
            if (player == null) { return; }
        }

        ChangeMaterial();
    }

    private void ChangeMaterial()
    {
        //用相机位置与角色位置的差值-1是为了避免检测到地面，提高可移植性，否则射线检测到的对象还要除去地面
        hits = new List<RaycastHit>(Physics.RaycastAll(transform.position, transform.forward, Vector3.Distance(this.transform.position, player.transform.position) - 1));

        //替换材质
        for (int i = 0; i < hits.Count; i++)
        {
            //射线检测到的对象除去角色
            if (hits[i].collider.gameObject.name != player.name)
            {
                var hit = hits[i];
                int findIndex = changedInfos.FindIndex(item => item.obj == hit.collider.gameObject);
                var rendArray = hit.collider.gameObject.GetComponentsInChildren<Renderer>();

                if (rendArray.Length > 0)
                {
                    //没找到则添加
                    if (findIndex < 0)
                    {
                        var changed = new HitInfo();
                        changed.obj = hit.collider.gameObject;
                        changed.renderers = rendArray;
                        changed.materials = new List<Material>();

                        for (int j = 0; j < rendArray.Length; j++)
                        {
                            var materials = rendArray[j].materials;
                            var tempMaterials = new Material[materials.Length];

                            for (int k = 0; k < materials.Length; k++)
                            {
                                changed.materials.Add(materials[k]);
                                tempMaterials[k] = alphaMaterial;
                            }

                            rendArray[j].materials = tempMaterials;//替换材质
                        }

                        changedInfos.Add(changed);
                    }
                }
            }
        }

        //还原材质
        for (int i = 0; i < changedInfos.Count;)
        {
            var changedInfo = changedInfos[i];
            var findIndex = hits.FindIndex(item => item.collider.gameObject == changedInfo.obj);

            //没找到则移除
            if (findIndex < 0)
            {
                if (changedInfo.obj != null)
                {
                    foreach (var renderer in changedInfo.renderers)
                    {
                        renderer.materials = changedInfo.materials.ToArray();//还原材质
                    }
                }

                changedInfos.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
}
