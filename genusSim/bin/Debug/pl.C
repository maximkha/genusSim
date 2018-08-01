void pl()
{
    ifstream fin("genetic.txt");
    vector<double> v_score;
    vector<double> v_p0;
    vector<double> v_p1;
    while(fin) {
        double score, p0, p1;
        fin >> score >> p0 >> p1;
        if (fin.eof()) break;
        v_score.push_back(score);
        v_p0.push_back(p0);
        v_p1.push_back(p1);
    } 
    cout << "Read " << v_score.size() << " points" <<  endl;

    TCanvas* c1 = new TCanvas("c1","c1");
    TGraph* gr = new TGraph(v_p0.size(),&v_p0[0],&v_p1[0]);
    gr->Draw("al");
    
    double xmin = 1e9,  xmax = -1e9, ymin = 1e9, ymax = -1e9;
    for (int i = 0; i<v_score.size(); ++i) {
        if (xmin > v_p0[i]) xmin = v_p0[i];
        if (xmax < v_p0[i]) xmax = v_p0[i];
        if (ymin > v_p1[i]) ymin = v_p1[i];
        if (ymax < v_p1[i]) ymax = v_p1[i];
    }
    ifstream fin1("solution.txt");
    double xsol, ysol;
    fin1 >> xsol >> ysol;
    TLine *l1 = new TLine(xsol, ymin, xsol, ymax);
    TLine* l2 = new TLine(xmin, ysol, xmax, ysol);
    l1->SetLineWidth(2);
    l1->SetLineColor(2);
    l1->Draw();
    l2->SetLineWidth(2);
    l2->SetLineColor(2);
    l2->Draw();

    c1->Modified();

    TH2* h = new TH2F("h","",100,xmin,xmax,100,ymin,ymax);
    TH2* hw = (TH2*)h->Clone("hw");
    for (int i = 0; i<v_score.size(); ++i) {
        h->Fill(v_p0[i],v_p1[i]);
        hw->Fill(v_p0[i],v_p1[i],v_score[i]);
    }
    h->Divide(hw,h);

    double zmin = 0;
    for (int i = 0; i<v_score.size(); ++i) {
        if (zmin==0 || (v_score[i]!=0 && zmin>v_score[i])) zmin = v_score[i];
    }
    h->SetMinimum(zmin);

    gStyle->SetOptStat(0);
    gStyle->SetPalette(1);
    TCanvas* c2 = new TCanvas("c2","c2");
    h->Draw("colz");
    c2->Modified();
}