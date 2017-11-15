using System;
using System.Collections;
using System.Collections.Generic;

public class JobQueue {

	public List<Job> pendingJobs { get; protected set; }
	public Queue<Job> availableJobs { get; protected set; }

	public List<Supply> deliverables; // rethink access

	public JobQueue() {
		pendingJobs = new List<Job>();
		availableJobs = new Queue<Job>();
		deliverables = new List<Supply>();
	}

	public void enqueue(Job job) {
		job.registerSupplyDeliveredCallback(supplyDelivered);
		job.registerJobFinishedCallback(jobFinished);

		pendingJobs.Add(job);
		deliverables.Add(job.supply);
	}

	public Job dequeue() {
		return availableJobs.Dequeue();
	}

	void supplyDelivered(Job job) {
		deliverables.Remove(job.supply);
		pendingJobs.Remove(job);
		availableJobs.Enqueue(job);
	}

	// Is this required??
	void jobFinished(Job job) {
		job.unregisterSupplyDeliveredCallback(supplyDelivered);
	}

	void jobAbandoned(Job job) {
		availableJobs.Enqueue(job);
	}
}

