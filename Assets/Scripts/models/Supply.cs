using System;
using System.Collections;
using System.Collections.Generic;

public class Supply {

	public Tile tile { get; protected set; }
	public string type { get; protected set; }

	Action<Supply> cbSupplyDelivered;

	public Supply(string type, Action<Supply> cbSupplyDelivered) {
		this.type = type;
		this.cbSupplyDelivered = cbSupplyDelivered;
	}

	public void setTile(Tile t) {
		tile = t;
		tile.hasSupply = true;
		if (cbSupplyDelivered != null) {
			cbSupplyDelivered(this);
		}
	}

	public void registerSupplyDeliveredCallback(Action<Supply> callback) {
		cbSupplyDelivered += callback;
	}

	public void unregisterSupplyDeliveredCallback(Action<Supply> callback) {
		cbSupplyDelivered -= callback;
	}
}
