﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job {

	public Tile tile { get; protected set; }
	public float time { get; protected set; }

	public Supply supply;

	Action<Job> cbSupplyReady;
	Action<Job> cbJobFinished;
	Action<Job> cbJobCancelled;

	public Job(Tile tile, float time, string supplyType, Action<Job> cbJobFinished) {
		this.tile = tile;
		this.time = time;
		this.cbJobFinished = cbJobFinished;

		supply = new Supply(supplyType, supplyDelivered);
	}

	public void doJob(float workTime) {
		time -= workTime;

		if (time < 0) {
			if (cbJobFinished != null) {
				cbJobFinished(this);
			}
		}
	}

	public void cancel() {
		if (cbJobCancelled != null)
			cbJobCancelled(this);
	}

	void supplyDelivered(Supply supply) {
		if (cbSupplyReady != null) {
			cbSupplyReady(this);
		}
	}

	public void registerSupplyReadyCallback(Action<Job> callback) {
		cbSupplyReady += callback;
	}

	public void unregisterSupplyReadyCallback(Action<Job> callback) {
		cbSupplyReady -= callback;
	}

	public void registerJobCancelledCallback(Action<Job> callback) {
		cbJobCancelled += callback;
	}

	public void unregisterJobCancelledCallback(Action<Job> callback) {
		cbJobCancelled -= callback;
	}

	public void registerJobFinishedCallback(Action<Job> callback) {
		cbJobFinished += callback;
	}

	public void unregisterJobFinishedCallback(Action<Job> callback) {
		cbJobFinished -= callback;
	}

}