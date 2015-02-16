using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

[System.Serializable]
public struct ElementConnection {
    public Element destinationElement;
    public Node originNode;
    public Node destinationNode;

    public ElementConnection(Element destinationElement, Node originNode, Node destinationNode) {
        this.destinationElement = destinationElement;
        this.originNode = originNode;
        this.destinationNode = destinationNode;
    }
}

public class ConnectableElement : Element {
    public List<ElementConnection> connections = new List<ElementConnection>();

    public void OnDestroy() {
        foreach (ElementConnection connection in connections) {
            if (connection.destinationElement != null) {
                DestroyImmediate(connection.destinationElement);
            }
            if (connection.destinationNode != null) {
                DestroyImmediate(connection.destinationNode);
            }
            if (connection.originNode != null) {
                DestroyImmediate(connection.destinationNode);
            }
        }
    }

    public virtual bool canConnectTo(ElementConnection connection) {
        return false;
    }

    public virtual bool tryConnect(ElementConnection connection) {
        if (!canConnectTo(connection)) {
            Debug.Log(this);
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