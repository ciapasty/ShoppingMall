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
		if (job.supply != null) {
			job.registerSupplyDeliveredCallback(supplyDelivered);
			job.registerJobFinishedCallback(jobFinished);
			job.registerJobCancelledCallback(jobCanceled);

			pendingJobs.Add(job);
			deliverables.Add(job.supply);
		} else {
			availableJobs.Enqueue(job);
		}
	}

	public Job dequeue() {
		return availableJobs.Dequeue();
	}

	void supplyDelivered(Job job) {
		deliverables.Remove(job.supply);
		pendingJobs.Remove(job);
		availableJobs.Enqueue(job);
	}

	public void jobUnreachable(Job job) {
		availableJobs.Enqueue(job);
	}

	// Is this required??
	void jobFinished(Job job) {
		job.unregisterSupplyDeliveredCallback(supplyDelivered);
	}

	void jobCanceled(Job job) {
		if (pendingJobs.Contains(job)) {
			pendingJobs.Remove(job);
			if (job.supply != null && deliverables.Contains(job.supply))
				deliverables.Remove(job.supply);
		}

		if (availableJobs.Contains(job)) {
			List<Job> jobList = new List<Job>(availableJobs.ToArray());
			jobList.Remove(job);
			availableJobs = new Queue<Job>(jobList);
		}

		job.unregisterSupplyDeliveredCallback(supplyDelivered);
		job.unregisterJobCancelledCallback(jobCanceled);
	}

	void jobAbandoned(Job job) {
		availableJobs.Enqueue(job);
	}
}

