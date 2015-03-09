using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

[System.Serializable]
public struct ElementConnection {
    public Element connectedElement;
    public Node originNode;
    public Node connectedNode;

    public ElementConnection(Element e, Node o, Node n) {
        connectedElement = e;
        originNode = o;
        connectedNode = n;
    }
}

public class ConnectableElement : Element{
    public List<ElementConnection> connections = new List<ElementConnection>();

    public void OnDestroy() {
        foreach (ElementConnection connection in connections) {
            if (connection.connectedElement != null) {
                DestroyImmediate(connection.connectedElement);
            }
            if (connection.connectedNode != null) {
                DestroyImmediate(connection.connectedNode);
            }
            if (connection.originNode != null) {
                DestroyImmediate(connection.connectedNode);
            }
        }
    }

    public override bool validate() {
        if (!base.validate()) {
            return false;
        }

        for (int i = connections.Count - 1; i >= 0; i--) {
            ElementConnection c = connections[i];
            if (c.connectedNode == null || c.connectedElement == null) {
                connections.RemoveAt(i);
                continue;
            }

            if (c.originNode == null) {
                c.originNode = parentNode;
            }

            connections[i] = c;
        }

        return true;
    }

    public virtual bool canConnectTo(ElementConnection connection) {
        return false;
    }

    public virtual bool tryConnect(ElementConnection connection) {
        if (!canConnectTo(connection)) {
            return false;
        }

        //If the connection is already in the list of connections we remove it
        //By placing the connection on the top we allow the user to control the order
        //While also keeping the relative order of all other connections
        connections.Remove(connection);

        if (connections.Count == getMaxConnections()) {
            disconnectFrom(connections[0]);
        }

        connections.Add(connection);

        return true;
    }

    public virtual void disconnectFrom(ElementConnection other) {
        connections.Remove(other);
    }

    public virtual void disconnectFromAll() {
        connections.Clear();
    }

    public virtual bool isConnected() {
        return connections.Count != 0;
    }
}

}