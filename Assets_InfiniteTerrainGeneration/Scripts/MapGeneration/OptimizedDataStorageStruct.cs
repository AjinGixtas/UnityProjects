public struct VoronoiPointData {
    public int xPos, yPos;
    public int pointIndex;
    public int pointIndexInArray;
    public VoronoiPointData(int xPos, int yPos, int pointIndex, int pointIndexInArray) {
        this.xPos = xPos;
        this.yPos = yPos;
        this.pointIndex = pointIndex;
        this.pointIndexInArray = pointIndexInArray;
    }
}