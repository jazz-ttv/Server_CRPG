// ============================================================
// ResourceSO
// ============================================================

function ResourceSO::addResources(%this)
{
	//makes sure it's clear when this is re-exec'd
	for(%a = 0; isObject(ResourceSO.resource[%a]); %a++)
	{
		ResourceSO.resource[%a].delete();
		ResourceSO.resource[%a] = "";
	}

	ResourceSO.mineralCount = 6;
	ResourceSO.treeCount = 6;

	ResourceSO.mineral[1] = new scriptObject() {
		id = 1;
		name = "Dirt";
		totalHits = 8;
		BPH = 0.14;
		color = "0.46 0.32 0.22 1";
		respawnMultiplier = 2.5; };

	ResourceSO.mineral[2] = new scriptObject() {
		id = 2;
		name = "Tin";
		totalHits = 12;
		BPH = 0.20;
		color = "0.71 0.65 0.57 1";
		respawnMultiplier = 1; };

	ResourceSO.mineral[3] = new scriptObject() {
		id = 3;
		name = "Copper";
		totalHits = 24;
		BPH = 0.22;
		color = "0.82 0.34 0 1";
		respawnMultiplier = 1; };

	ResourceSO.mineral[4] = new scriptObject() {
		id = 4;
		name = "Iron";
		totalHits = 28;
		BPH = 0.25;
		color = "0.31 0.29 0.25 1";
		respawnMultiplier = 1; };

	ResourceSO.mineral[5] = new scriptObject() {
		id = 5;
		name = "Silver";
		totalHits = 32;
		BPH = 0.3;
		color = "0.9 0.9 0.9 1";
		respawnMultiplier = 1; };

	ResourceSO.mineral[6] = new scriptObject() {
		id = 6;
		name = "Gold";
		totalHits = 40;
		BPH = 0.35;
		color = "0.9 0.9 0 1";
		respawnMultiplier = 1; };

	ResourceSO.tree[1] = new scriptObject() {
		id = 1;
		name = "Pine";
		BPH = 0.21;
		TotalHits = 12;
		 Color = "0.12 0.32 0.05 1"; };

	ResourceSO.tree[2] = new scriptObject() {
		id = 2;
		name = "Dead";
		BPH = 0.16;
		TotalHits = 8;
		 Color = "0.18 0.12 0.07 1"; };

	ResourceSO.tree[3] = new scriptObject() {
		id = 3;
		name = "Oak";
		BPH = 0.20;
		TotalHits = 24;
		Color = "0.47 0.32 0.22 1"; };

	ResourceSO.tree[4] = new scriptObject() {
		id = 3;
		name = "Maple";
		BPH = 0.22;
		TotalHits = 16;
		Color = "0.58 0.22 0 1"; };
	
	ResourceSO.tree[5] = new scriptObject() {
		id = 3;
		name = "Chestnut";
		BPH = 0.25;
		TotalHits = 20;
		Color = "0.34 0.13 0.12 1"; };

	ResourceSO.tree[6] = new scriptObject() {
		id = 3;
		name = "Walnut";
		BPH = 0.35;
		TotalHits = 28;
		Color = "0.57 0.41 0.06 1"; };
}

if(isObject(ResourceSO))
	ResourceSO.delete();

if(!isObject(ResourceSO))
{
	new scriptObject(ResourceSO) { };
	ResourceSO.addResources();
}